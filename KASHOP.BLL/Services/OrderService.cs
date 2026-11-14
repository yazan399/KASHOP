using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Models;
using KASHOP.DAL.Dto;
using Mapster;
using System.Linq.Expressions;
using KASHOP.DAL.Repository;
using System.Globalization;
using KASHOP.DAL.Enums;
using KASHOP.BLL.Extensions;

namespace KASHOP.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<Result<paginationResponse<OrderSummaryResponse>>> GetUserOrdersAsync(string userId, OrderQueryRequest queryRequest)
        {
            var query = BuildOrderQuery(queryRequest, o => o.UserId == userId);
            var page = await query.ToPaginateAsync(queryRequest.PageNumber, queryRequest.PageSize);
            return Result<paginationResponse<OrderSummaryResponse>>.Ok(new paginationResponse<OrderSummaryResponse>
            {
                Data = page.Data.Adapt<List<OrderSummaryResponse>>(),
                TotalCount = page.TotalCount,
                PageNumber = page.PageNumber,
                PageSize = page.PageSize
            });
        }



        public async Task<Result<OrderDetailsResponse>> GetUserOrderDetailsAsync(string userId, string orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetOneAsync(
                o => o.UserId == userId && o.Id.ToString() == orderId,
                new[]
                {
                    nameof(Order.OrderItems),
                    $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}",
                    $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}.{nameof(Product.Translations)}"
                });
            if (order == null)
            {
                return Result<OrderDetailsResponse>.Fail("Order not found");
            }
            var orderDetails = order.Adapt<OrderDetailsResponse>();
            return Result<OrderDetailsResponse>.Ok(orderDetails);
        }

        public async Task<Result<OrderDetailsResponse>> GetOrderDetailsAsync(string orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetOneAsync(
                o => o.Id.ToString() == orderId,
                [nameof(Order.OrderItems), $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}", $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}.{nameof(Product.Translations)}", nameof(Order.User)]);
            return order == null
                ? Result<OrderDetailsResponse>.Fail("Order not found")
                : Result<OrderDetailsResponse>.Ok(order.Adapt<OrderDetailsResponse>());
        }

        public async Task<Result<paginationResponse<OrderDetailsResponse>>> GetAllOrdersAsync(OrderQueryRequest queryRequest)
        {
            var query = BuildOrderQuery(queryRequest);
            var page = await query.ToPaginateAsync(queryRequest.PageNumber, queryRequest.PageSize);
            return Result<paginationResponse<OrderDetailsResponse>>.Ok(new paginationResponse<OrderDetailsResponse>
            {
                Data = page.Data.Adapt<List<OrderDetailsResponse>>(),
                TotalCount = page.TotalCount,
                PageNumber = page.PageNumber,
                PageSize = page.PageSize
            });
        }

        private IQueryable<Order> BuildOrderQuery(OrderQueryRequest request, Expression<Func<Order, bool>>? additionalFilter = null)
        {
            var query = _unitOfWork.OrderRepository.GetQueryable(additionalFilter, [nameof(Order.OrderItems), nameof(Order.User)]);
            if (request.Status.HasValue) query = query.Where(o => o.Status == request.Status.Value);
            if (request.PaymentStatus.HasValue) query = query.Where(o => o.PaymentStatus == request.PaymentStatus.Value);
            if (request.PaymentMethod.HasValue) query = query.Where(o => o.PaymentMethod == request.PaymentMethod.Value);
            if (request.FromDate.HasValue) query = query.Where(o => o.OrderDate >= request.FromDate.Value);
            if (request.ToDate.HasValue) query = query.Where(o => o.OrderDate <= request.ToDate.Value);
            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(o => o.Id.ToString().Contains(request.Search) || o.PhoneNumber.Contains(request.Search) || o.City.Contains(request.Search) || o.User.UserName!.Contains(request.Search));

            return (request.SortBy, request.SortDescending) switch
            {
                (OrderSortBy.Id, false) => query.OrderBy(o => o.Id),
                (OrderSortBy.Id, true) => query.OrderByDescending(o => o.Id),
                (OrderSortBy.TotalAmount, false) => query.OrderBy(o => o.TotalAmount),
                (OrderSortBy.TotalAmount, true) => query.OrderByDescending(o => o.TotalAmount),
                (OrderSortBy.Status, false) => query.OrderBy(o => o.Status),
                (OrderSortBy.Status, true) => query.OrderByDescending(o => o.Status),
                (OrderSortBy.PaymentStatus, false) => query.OrderBy(o => o.PaymentStatus),
                (OrderSortBy.PaymentStatus, true) => query.OrderByDescending(o => o.PaymentStatus),
                (OrderSortBy.OrderDate, false) => query.OrderBy(o => o.OrderDate),
                _ => query.OrderByDescending(o => o.OrderDate)
            };
        }

    
    public async Task<Result<OrderDetailsResponse>> UpdateOrderStatusAsync(string orderId, OrderStatusEnum newStatus)
        {
            var order = await _unitOfWork.OrderRepository.GetOneAsync(o => o.Id.ToString() == orderId);
            if (order == null)
            {
                return Result<OrderDetailsResponse>.Fail("Order not found");
            }
            if (order.Status == newStatus)
            {
                return Result<OrderDetailsResponse>.Fail($"Order is already {newStatus}");
            }
            if (order.PaymentMethod == PaymentMethodEnum.Visa && order.PaymentStatus is not PaymentStatusEnum.Paid)
            {
                return Result<OrderDetailsResponse>.Fail("Visa order is not paid yet");
            }
            bool isAllowed = false;
            if(order.Status == OrderStatusEnum.Pending && newStatus == OrderStatusEnum.Confirmed)
            {
                isAllowed = true;
            }
            else if (order.Status == OrderStatusEnum.Confirmed && newStatus == OrderStatusEnum.Preparing)
            {
                isAllowed = true;
            }
            else if (order.Status == OrderStatusEnum.Preparing && newStatus == OrderStatusEnum.Delivered)
            {
                isAllowed = true;
            }
            else if (newStatus == OrderStatusEnum.Canceled)
            {
                isAllowed = true;
            }
            if (!isAllowed)
            {
                return Result<OrderDetailsResponse>.Fail($"Cannot update order status from {order.Status} to {newStatus}");
            }

            order.Status = newStatus;
            if (newStatus == OrderStatusEnum.Delivered)
            {
                order.ShippedDate = DateTime.UtcNow;
                if (order.PaymentMethod == PaymentMethodEnum.Cash)
                {
                    order.PaymentStatus = PaymentStatusEnum.Paid;
                    order.PaidAt = DateTime.UtcNow;
                    order.AmountPaid = order.TotalAmount;
                }
            }


            await _unitOfWork.CompleteAsync();
            return Result<OrderDetailsResponse>.Ok(order.Adapt<OrderDetailsResponse>());
        }
    }
}

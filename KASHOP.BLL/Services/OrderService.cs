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

namespace KASHOP.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<Result<List<OrderSummaryResponse>>> GetUserOrdersAsync(string userId)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(o => o.UserId == userId, new[] { nameof(Order.OrderItems) });
            if (orders == null || !orders.Any())
            {
                return Result<List<OrderSummaryResponse>>.Fail("No orders found for the user");
            }
            var response = orders.OrderByDescending(o => o.OrderDate).Adapt<List<OrderSummaryResponse>>();

            return Result<List<OrderSummaryResponse>>.Ok(response);
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

        public async Task<Result<List<OrderDetailsResponse>>> GetAllOrdersAsync(OrderStatusEnum? status)
        {
            Expression<Func<Order, bool>> filter = null;
            if (status.HasValue)
            {
                filter = o => o.Status == status.Value;
            }
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(filter, new[] { nameof(Order.OrderItems) });
            var response = orders?
                .OrderByDescending(o => o.OrderDate)
                .Adapt<List<OrderDetailsResponse>>() ?? new List<OrderDetailsResponse>();
            return Result<List<OrderDetailsResponse>>.Ok(response);
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

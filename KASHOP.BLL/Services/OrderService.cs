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

        public async Task<Result<OrderDetailsResponse>> GetAllOrdersAsync(OrderStatusEnum? status)
        {
            Expression<Func<Order, bool>> filter = null;
            if (status.HasValue)
            {
                filter = o => o.Status == status.Value;
            }
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(filter, new[] { nameof(Order.OrderItems) });
            if (orders == null || !orders.Any())
            {
                return Result<OrderDetailsResponse>.Fail("No orders found");
            }
            var response = orders.OrderByDescending(o => o.OrderDate).Adapt<List<OrderDetailsResponse>>();
            return Result<OrderDetailsResponse>.Ok(response.FirstOrDefault());
        }

    }
}

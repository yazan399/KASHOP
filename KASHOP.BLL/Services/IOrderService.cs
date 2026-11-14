using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KASHOP.DAL.Dto;
using System.Threading.Tasks;
using KASHOP.DAL.Enums;

namespace KASHOP.BLL.Services
{
    public interface IOrderService
    {

        public Task<Result<paginationResponse<OrderSummaryResponse>>> GetUserOrdersAsync(string userId, OrderQueryRequest queryRequest);
        public Task<Result<OrderDetailsResponse>> GetUserOrderDetailsAsync(string userId, string orderId);
        public Task<Result<OrderDetailsResponse>> GetOrderDetailsAsync(string orderId);

        public Task<Result<paginationResponse<OrderDetailsResponse>>> GetAllOrdersAsync(OrderQueryRequest queryRequest);

        public Task<Result<OrderDetailsResponse>> UpdateOrderStatusAsync(string orderId, OrderStatusEnum newStatus);


    }
}

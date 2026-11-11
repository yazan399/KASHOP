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

        public Task<Result<List<OrderSummaryResponse>>> GetUserOrdersAsync(string userId);
        public Task<Result<OrderDetailsResponse>> GetUserOrderDetailsAsync(string userId, string orderId);

        public Task<Result<OrderDetailsResponse>> GetAllOrdersAsync(OrderStatusEnum? status);
            
        
    }
}

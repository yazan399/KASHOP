using System;
using System.Collections.Generic;
using System.Linq;
using KASHOP.DAL.Models;
using KASHOP.DAL.Dto;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public interface ICheckoutService
    {
        public Task<Result<OrderResponse>> CheckoutAsync(string userId, CheckoutRequest request);
    }
}

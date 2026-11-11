using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KASHOP.DAL.Dto;
using System.Security.Claims;
using Mapster;
using KASHOP.DAL.Models;
using KASHOP.DAL.Data;
using Microsoft.EntityFrameworkCore;
using KASHOP.DAL.Repository;
using KASHOP.BLL.Services;
using Microsoft.AspNetCore.Authorization;

using Microsoft.Extensions.Localization;
using KASHOP.DAL.Enums;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IOrderService _orderService;
        private readonly ICheckoutService _checkoutService;
        public OrdersController(IStringLocalizer<SharedResources> localizer, ICheckoutService checkoutService, IOrderService orderService)
        {
            _localizer = localizer;
            _checkoutService = checkoutService;
            _orderService = orderService;
        }

        [HttpPost("checkout")]


             public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)

             {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized(new { Message = _localizer["Unauthorized"].Value });
                }
                var result = await _checkoutService.CheckoutAsync(userId, request);

                if (!result.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = result.Message });
                }
                return Ok(new { Data = result.Data, Message = _localizer["Success"].Value });
             }

        [HttpGet("payment/success")]
        [AllowAnonymous]
        public async Task<IActionResult> Payment(string sessionId)
        {
            var result = await _checkoutService.HandleSuccess(sessionId);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = result.Message });
            }
            return Ok(new { Data = result.Data, Message = _localizer["PaymentSuccessful"].Value });
        }


        [HttpGet("payment/cancel")] // i must do this later
        [AllowAnonymous]
        public async Task<IActionResult> Cancel(string sessionId)
        {
            return Ok(new { Message = _localizer["PaymentCancelled"].Value });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = _localizer["Unauthorized"].Value });
            }
            var result = await _orderService.GetUserOrdersAsync(userId);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = result.Message });
            }
            return Ok(new { Data = result.Data, Message = _localizer["Success"].Value });
        }
        
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetUserOrderDetails(string orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = _localizer["Unauthorized"].Value });
            }
            var result = await _orderService.GetUserOrderDetailsAsync(userId, orderId);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = result.Message });
            }
            return Ok(new { Data = result.Data, Message = _localizer["Success"].Value });
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderStatusEnum? status=OrderStatusEnum.Pending)
        {
            var result = await _orderService.GetAllOrdersAsync(status);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = result.Message });
            }
            return Ok(new { Data = result.Data, Message = _localizer["Success"].Value });
        }
    }
}

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

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;

        private readonly ICheckoutService _checkoutService;
        public OrdersController(IStringLocalizer<SharedResources> localizer, ICheckoutService checkoutService)
        {
            _localizer = localizer;
            _checkoutService = checkoutService;
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

    }
}

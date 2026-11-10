using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using KASHOP.PL.Resources;
using KASHOP.DAL.Dto;
using System.Security.Claims;
using Mapster;
using KASHOP.DAL.Models;
using KASHOP.DAL.Data;
using Microsoft.EntityFrameworkCore;
using KASHOP.DAL.Repository;
using KASHOP.BLL.Services;
using Microsoft.AspNetCore.Authorization;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IStringLocalizer<SharedResources> _localizer;
        public CartsController(ICartService cartService, IStringLocalizer<SharedResources> localizer)
        {
            _cartService = cartService;
            _localizer = localizer;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartItemRequest cartRequest)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = _localizer["Unauthorized"].Value });
            }
            var result = await _cartService.AddToCart(userId, cartRequest);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = result.Message });
            }
            return Ok(new { Message = _localizer["AddedToCart"].Value });
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = _localizer["Unauthorized"].Value });
            }
            var result = await _cartService.GetCartItemsAsync(userId);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = result.Message });
            }
            return Ok(new { Data = result.Data, Message = _localizer["Success"].Value });
        }
       

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromCart([FromRoute] int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = _localizer["Unauthorized"].Value });
            }
            var result = await _cartService.RemoveFromCart(userId, productId);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = result.Message });
            }
            return Ok(new { Message = _localizer["RemovedFromCart"].Value });
        }


        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateQuantity(int productId, [FromBody] int newCount)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = _localizer["Unauthorized"].Value });
            }
            var result = await _cartService.UpdateQuantity(userId, productId, newCount);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = result.Message });
            }
            return Ok(new { Message = _localizer["UpdatedCart"].Value });
        }


        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = _localizer["Unauthorized"].Value });
            }
            var result = await _cartService.ClearCart(userId);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = result.Message });
            }
            return Ok(new { Message = _localizer["ClearedCart"].Value });
        }

    }
}

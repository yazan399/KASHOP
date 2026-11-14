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
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public ProductsController(IProductService productService, IStringLocalizer<SharedResources> localizer)
        {
            _productService = productService;
            _localizer = localizer;
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Super Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductRequest productRequest)
        {
            var result = await _productService.CreateProductAsync(productRequest);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }
            return Ok(new { Data = result.Data, Message = _localizer["Success"].Value });
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryRequest queryRequest)
        {
            var result = await _productService.GetAllProductsAsync(queryRequest);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = result.Message });
            }
            return Ok(new { Data = result.Data, Message = _localizer["Success"].Value });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            var result = await _productService.GetProduct(p => p.Id == id);
            if (!result.Success)
            {
                return NotFound(new { Message = result.Message });
            }
            return Ok(new { Data = result.Data, Message = _localizer["Success"].Value });
        }

        [HttpGet("name/{name}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductByName([FromRoute] string name)
        {
            var result = await _productService.GetProduct(p => p.Translations.Any(t => t.Name == name));
            if (!result.Success)
            {
                return NotFound(new { Message = result.Message });
            }
            return Ok(new { Data = result.Data, Message = _localizer["Success"].Value });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Super Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = result.Message });
            }
            return Ok(new { Data = result.Data, Message = _localizer["Success"].Value });
        }
    }
}

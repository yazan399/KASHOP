using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using KASHOP.PL.Resources;
using KASHOP.DAL.Dto;
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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IStringLocalizer<SharedResources> _localizer;
        public CategoriesController( ICategoryService categoryService, IStringLocalizer<SharedResources> localizer)
        {
            _categoryService = categoryService;
            _localizer = localizer;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _categoryService.GetAllCategoriesAsync();
            return Ok(new { Data = response, Message = _localizer["Success"].Value });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneById(int id)
        {
            var response = await _categoryService.GetOneCategoryAsync(c => c.Id == id);
            if (response == null)
            {
                return NotFound(new { Message = _localizer["NotFound"].Value });
            }
            return Ok(new { Data = response, Message = _localizer["Success"].Value });
        }
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetOneByName(string name)
        {
            var response = await _categoryService.GetOneCategoryAsync(c => c.Translations.Any(t => t.Name == name));
            if (response == null)
            {
                return NotFound(new { Message = _localizer["NotFound"].Value });
            }
            return Ok(new { Data = response, Message = _localizer["Success"].Value });
        }

        // so always i will get the data from client as dto and then i will make it as domain model to save it in db and when i will send the data to client i will make it as dto to send it to client 

        [HttpPost]
        public async Task<IActionResult> Create(CategoryRequest request)
        {
            var response = await _categoryService.CreateCategoryAsync(request);

            return Ok(new
            {
                Data = response,
                Message = _localizer["Success"].Value
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _categoryService.DeleteCategoryAsync(id);
            return Ok(new { Data = response, Message = _localizer["Success"].Value });
        }
    }
}
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

namespace KASHOP.PL.Controllers


{
    [Route("api/[controller]")]
    [ApiController]
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
        public IActionResult Index()
        {
            var response = _categoryService.GetAllCategories();
            return Ok(new { Data = response, Message = _localizer["Success"].Value });
        }
        // so always i will get the data from client as dto and then i will make it as domain model to save it in db and when i will send the data to client i will make it as dto to send it to client 

        [HttpPost]
        public IActionResult Create(CategoryRequest request)
        {
            var response = _categoryService.CreateCategory(request);

            return Ok(new
            {
                Data = response,
                Message = _localizer["Success"].Value
            });
        }
    }
}
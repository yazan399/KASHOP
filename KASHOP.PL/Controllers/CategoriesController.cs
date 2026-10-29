using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using KASHOP.PL.Resources;
using KASHOP.DAL.Dto;
using Mapster;
using KASHOP.DAL.Models;
using KASHOP.DAL.Data;
using Microsoft.EntityFrameworkCore;
namespace KASHOP.PL.Controllers


{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CategoriesController(ApplicationDbContext context, IStringLocalizer<SharedResources> localizer)
        {
            _context = context;
            _localizer = localizer;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var categories = _context.Categories.Include(c => c.Translations).ToList();
            var response = categories.Adapt<List<CategoryResponse>>();
            return Ok(new { Data = response, Message = _localizer["Success"].Value });
        }


        [HttpPost]
        public IActionResult Create(CategoryRequest request)
        {
            var category = request.Adapt<Category>();
            _context.Add(category);
            _context.SaveChanges();
            return Ok(_localizer["Success"].Value);
            
        }
    }
}
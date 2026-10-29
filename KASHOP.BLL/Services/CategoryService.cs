using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Models;
using KASHOP.DAL.Dto;
using Mapster;
using KASHOP.DAL.Repository;
namespace KASHOP.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<List<CategoryResponse>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllCategories();
            var response = categories.Adapt<List<CategoryResponse>>();
            return response;
        }
        public async Task<CategoryResponse> CreateCategory(CategoryRequest categoryRequest)
        {
            var category = categoryRequest.Adapt<Category>();
            var createdCategory = await _categoryRepository.CreateCategory(category);
            var response = createdCategory.Adapt<CategoryResponse>();
            return response;
        }
    }
}

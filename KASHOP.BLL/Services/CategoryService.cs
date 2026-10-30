using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Models;
using KASHOP.DAL.Dto;
using Mapster;
using System.Linq.Expressions;
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
        public async Task<List<CategoryResponse>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync(new string[] { nameof(Category.Translations) });
            var response = categories.Adapt<List<CategoryResponse>>();
            return response;
        }
        public async Task<CategoryResponse> GetOneAsync(Expression<Func<Category, bool>> filter)
        {
            var category = await _categoryRepository.GetOneAsync(filter, new string[] { nameof(Category.Translations) });
            var response = category.Adapt<CategoryResponse>();
            return response;
        }
        public async Task<CategoryResponse> CreateAsync(CategoryRequest categoryRequest)
        {
            var category = categoryRequest.Adapt<Category>();
            var createdCategory = await _categoryRepository.CreateAsync(category);
            var response = createdCategory.Adapt<CategoryResponse>();
            return response;
        }

        public async Task<CategoryResponse> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            var deletedCategory = await _categoryRepository.DeleteAsync(category);
            return deletedCategory.Adapt<CategoryResponse>();
        }

    }
}

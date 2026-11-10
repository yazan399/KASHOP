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
using System.Globalization;
namespace KASHOP.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork    ;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<List<CategoryResponse>>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync(includes: new string[] { nameof(Category.Translations), nameof(Category.CreatedBy) });
            var response = categories.Adapt<List<CategoryResponse>>();
            return Result<List<CategoryResponse>>.Ok(response);
        }
        public async Task<Result<CategoryResponse>> GetOneCategoryAsync(
            Expression<Func<Category, bool>> filter)
        {
            var category = await _unitOfWork.CategoryRepository.GetOneAsync(
                filter,
                new[]
                {
                    nameof(Category.Translations),
                    nameof(Category.CreatedBy)
                });

            if (category == null)
            {
                return Result<CategoryResponse>.Fail("Category not found");
            }

            var response = category.Adapt<CategoryResponse>();
            return Result<CategoryResponse>.Ok(response);
        }
        public async Task<Result<CategoryResponse>> CreateCategoryAsync(CategoryRequest categoryRequest)
        {
            var category = categoryRequest.Adapt<Category>();
            var createdCategory = await _unitOfWork.CategoryRepository.CreateAsync(category);
            await _unitOfWork.CompleteAsync();
            var creatoruser = await _unitOfWork.CategoryRepository.GetOneAsync(c => c.Id == createdCategory.Id, new string[] { nameof(Category.CreatedBy) });
            var response = createdCategory.Adapt<CategoryResponse>();
            return Result<CategoryResponse>.Ok(response);
        }

        public async Task<Result<CategoryResponse>> DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetOneAsync(c => c.Id == id);
            if (category == null)
            {
                return Result<CategoryResponse>.Fail("Category not found");
            }

           _unitOfWork.CategoryRepository.Delete(category);
            var affectedRows = await _unitOfWork.CompleteAsync();
            return Result<CategoryResponse>.Ok();
        }

    }
}


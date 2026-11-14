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
using KASHOP.BLL.Extensions;
namespace KASHOP.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork    ;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<paginationResponse<CategoryResponse>>> GetAllCategoriesAsync(CategoryQueryRequest queryRequest)
        {
            var query = _unitOfWork.CategoryRepository.GetQueryable(includes: [nameof(Category.Translations), nameof(Category.CreatedBy)]);
            if (!string.IsNullOrWhiteSpace(queryRequest.Search))
                query = query.Where(c => c.Translations.Any(t => t.Name.Contains(queryRequest.Search)));

            query = (queryRequest.SortBy, queryRequest.SortDescending) switch
            {
                (CategorySortBy.Name, false) => query.OrderBy(c => c.Translations.Select(t => t.Name).FirstOrDefault()),
                (CategorySortBy.Name, true) => query.OrderByDescending(c => c.Translations.Select(t => t.Name).FirstOrDefault()),
                (CategorySortBy.CreatedOn, false) => query.OrderBy(c => c.CreatedOn),
                (CategorySortBy.CreatedOn, true) => query.OrderByDescending(c => c.CreatedOn),
                (CategorySortBy.Id, false) => query.OrderBy(c => c.Id),
                _ => query.OrderByDescending(c => c.Id)
            };

            var page = await query.ToPaginateAsync(queryRequest.PageNumber, queryRequest.PageSize);
            return Result<paginationResponse<CategoryResponse>>.Ok(new paginationResponse<CategoryResponse>
            {
                Data = page.Data.Adapt<List<CategoryResponse>>(),
                TotalCount = page.TotalCount,
                PageNumber = page.PageNumber,
                PageSize = page.PageSize
            });
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
            if (categoryRequest.Translations == null || categoryRequest.Translations.Count == 0 ||
                categoryRequest.Translations.Any(t => string.IsNullOrWhiteSpace(t.Name) || string.IsNullOrWhiteSpace(t.Language)) ||
                categoryRequest.Translations.GroupBy(t => t.Language, StringComparer.OrdinalIgnoreCase).Any(g => g.Count() > 1))
            {
                return Result<CategoryResponse>.Fail("Provide one complete translation per language.");
            }
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


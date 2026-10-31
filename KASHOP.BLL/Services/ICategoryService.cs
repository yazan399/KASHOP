using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using KASHOP.DAL.Models;
using KASHOP.DAL.Dto;

namespace KASHOP.BLL.Services
{
    public interface ICategoryService
    {
        public Task<List<CategoryResponse>> GetAllCategoriesAsync();    
        public Task<CategoryResponse> CreateCategoryAsync(CategoryRequest categoryRequest);
        public Task<CategoryResponse> GetOneCategoryAsync(Expression<Func<Category, bool>> filter);
        public Task<CategoryResponse> DeleteCategoryAsync(int id);
    }
}

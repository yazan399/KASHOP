using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Models;

namespace KASHOP.DAL.Repository
{
    public interface ICategoryRepository
    {
       Task<List<Category>> GetAllCategories();
        Task<Category> CreateCategory(Category category);

    }
}

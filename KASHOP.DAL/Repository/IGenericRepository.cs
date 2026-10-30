using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
namespace KASHOP.DAL.Repository
{
    public interface IGenericRepository<T> where T : class
    {

        public Task<List<T>> GetAllAsync(string[]? includes=null);
        public Task<T> CreateAsync(T entity);
        public Task<T> GetOneAsync(Expression<Func<T, bool>> filter, string[]? includes = null);
        public Task<T> DeleteAsync(T entity);

    }

}

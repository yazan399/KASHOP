using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Models;
using Microsoft.EntityFrameworkCore;
using KASHOP.DAL.Repository;
using KASHOP.DAL.Data;
using System.Linq.Expressions;

namespace KASHOP.DAL.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

      
        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string[]? includes=null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            var entities = await query.ToListAsync();
            return entities;
        }

        public async Task<T> GetOneAsync(Expression<Func<T, bool>> filter, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            var entity = await query.FirstOrDefaultAsync(filter);
            return entity;

        }


        // This method returns an IQueryable<T> that can be further queried or paginated
        public IQueryable<T> GetQueryable(Expression<Func<T, bool>>? filter = null, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query;
        }


        public async Task<T> CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
            return entity;
        }
       

        public  void Delete(T entity)
        {
             _context.Remove(entity);
        }   
    }
}

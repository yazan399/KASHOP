using KASHOP.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Models;
using Microsoft.EntityFrameworkCore;
using KASHOP.DAL.Repository;
using KASHOP.DAL.Dto;

namespace KASHOP.DAL.Repository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

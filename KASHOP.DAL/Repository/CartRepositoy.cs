using System;
using System.Collections.Generic;
using System.Linq;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Data;

namespace KASHOP.DAL.Repository
{
    public class CartRepositoy : GenericRepository<CartItem>, ICartRepository
    {
        public CartRepositoy(ApplicationDbContext context) : base(context)
        {
        }
    
    }
}

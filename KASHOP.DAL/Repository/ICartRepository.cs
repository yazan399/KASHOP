using System;
using System.Collections.Generic;
using System.Linq;
using KASHOP.DAL.Models;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Repository
{
    public interface ICartRepository : IGenericRepository<CartItem>
    {
    }
}

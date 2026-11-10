using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using KASHOP.DAL.Models;
using KASHOP.DAL.Dto;


namespace KASHOP.BLL.Services
{
    public interface ICartService
    {
        public Task<Result<List<CartItemResponse>>> GetCartItemsAsync(string userId);
        public Task<Result<bool>> AddToCart(string userId, CartItemRequest cartRequest);
        public Task<Result<bool>> UpdateQuantity(string userId, int productId, int newCount);
        public Task<Result<bool>> RemoveFromCart(string userId, int productId);
        public Task<Result<bool>> ClearCart(string userId);
    }
}

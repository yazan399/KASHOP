using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using KASHOP.DAL.Models;
using KASHOP.DAL.Dto;


namespace KASHOP.BLL.Services
{
    public interface IProductService
    {
        public Task<Result<ProductResponse>> GetProduct(Expression<Func<Product, bool>> filter);
        public Task<Result<List<ProductResponse>>> GetAllProductsAsync();

        public Task<Result<bool>> DeleteProduct(int productId);
        public Task<Result<ProductResponse>> CreateProductAsync(ProductRequest productRequest);
    }
}

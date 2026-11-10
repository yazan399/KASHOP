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


namespace KASHOP.BLL.Services
{
    public class ProductService : IProductService
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        public ProductService( IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<Result<ProductResponse>> CreateProductAsync(ProductRequest productRequest)
        {
            if (productRequest.MainImage == null || productRequest.MainImage.Length == 0)
            {
                return Result<ProductResponse>.Fail("Main image is required.");
            }

            var uploadResult = await _fileService.UploadAsync(productRequest.MainImage);
            if (!uploadResult.Success)
            {
                return Result<ProductResponse>.Fail(uploadResult.Message);
            }

            if (string.IsNullOrWhiteSpace(uploadResult.Data.Url))
            {
                return Result<ProductResponse>.Fail("File upload did not return a valid image URL.");
            }

            if (string.IsNullOrWhiteSpace(uploadResult.Data.PublicId))
            {
                return Result<ProductResponse>.Fail("File upload did not return a valid public ID.");
            }
            var product = productRequest.Adapt<Product>();
            product.MainImage = uploadResult.Data.Url;
            product.MainImagePublicId = uploadResult.Data.PublicId;
            await _unitOfWork.ProductRepository.CreateAsync(product);
            await _unitOfWork.CompleteAsync();
            return Result<ProductResponse>.Ok(product.Adapt<ProductResponse>());
        }

        public async Task<Result<bool>> DeleteProduct(int productId)
        {
            var product = _unitOfWork.ProductRepository.GetOneAsync(p => p.Id == productId);
            if (product == null)  
            {
                return Result<bool>.Fail("Product not found.");
            }
            var deleteFileResult = await _fileService.Delete(product.Result.MainImagePublicId);
            if (!deleteFileResult.Success)
            {
                return Result<bool>.Fail(deleteFileResult.Message);
            }

            _unitOfWork.ProductRepository.Delete(product.Result);
           var completeResult = await _unitOfWork.CompleteAsync();
            return completeResult > 0 ? Result<bool>.Ok(true) : Result<bool>.Fail("Failed to delete product.");
        }



        public async Task<Result<List<ProductResponse>>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(includes: new string[] { nameof(Product.Translations), nameof(Product.Category) });
            if (products == null || !products.Any())
            {
                return Result<List<ProductResponse>>.Fail("No products found.");
            }

            var response = products.Adapt<List<ProductResponse>>();
            return Result<List<ProductResponse>>.Ok(response);
        }
        
        public async Task<Result<ProductResponse>> GetProduct(Expression<Func<Product, bool>> filter)
        {
            var product = await _unitOfWork.ProductRepository.GetOneAsync(filter, new string[] { nameof(Product.Translations), nameof(Product.Category) });
            if (product == null)
            {
                return Result<ProductResponse>.Fail("Product not found.");
            }

            return Result<ProductResponse>.Ok(product.Adapt<ProductResponse>());
        }
    }
}









using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Models;
using KASHOP.DAL.Dto;
using Mapster;
using System.Linq.Expressions;
using KASHOP.BLL.Extensions;
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
            var categoryExists = await _unitOfWork.CategoryRepository.GetOneAsync(c => c.Id == productRequest.CategoryId);
            if (categoryExists == null)
            {
                return Result<ProductResponse>.Fail("Category not found.");
            }
            if (productRequest.Translations == null || productRequest.Translations.Count == 0 ||
                productRequest.Translations.Any(t => string.IsNullOrWhiteSpace(t.Name) || string.IsNullOrWhiteSpace(t.Description) || string.IsNullOrWhiteSpace(t.Language)) ||
                productRequest.Translations.GroupBy(t => t.Language, StringComparer.OrdinalIgnoreCase).Any(g => g.Count() > 1))
            {
                return Result<ProductResponse>.Fail("Provide one complete translation per language.");
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
            var product = await _unitOfWork.ProductRepository.GetOneAsync(p => p.Id == productId);
            if (product == null)  
            {
                return Result<bool>.Fail("Product not found.");
            }
            var deleteFileResult = await _fileService.Delete(product.MainImagePublicId);
            if (!deleteFileResult.Success)
            {
                return Result<bool>.Fail(deleteFileResult.Message);
            }

            _unitOfWork.ProductRepository.Delete(product);
           var completeResult = await _unitOfWork.CompleteAsync();
            return completeResult > 0 ? Result<bool>.Ok(true) : Result<bool>.Fail("Failed to delete product.");
        }




        public async Task<Result<paginationResponse<ProductResponse>>> GetAllProductsAsync(ProductQueryRequest queryRequest)
        {
            var query = _unitOfWork.ProductRepository.GetQueryable(null, new string[] { nameof(Product.Translations), nameof(Product.Category) });

            if (!string.IsNullOrWhiteSpace(queryRequest.Search))
            {
                query = query.Where(p => p.Translations.Any(t => t.Name.Contains(queryRequest.Search)));
            }
            if (queryRequest.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == queryRequest.CategoryId.Value);
            if (queryRequest.MinPrice.HasValue)
                query = query.Where(p => p.Price >= queryRequest.MinPrice.Value);
            if (queryRequest.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= queryRequest.MaxPrice.Value);
            if (queryRequest.InStock.HasValue)
                query = queryRequest.InStock.Value ? query.Where(p => p.Quantity > 0) : query.Where(p => p.Quantity == 0);

            query = (queryRequest.SortBy, queryRequest.SortDescending) switch
            {
                (ProductSortBy.Name, false) => query.OrderBy(p => p.Translations.Select(t => t.Name).FirstOrDefault()),
                (ProductSortBy.Name, true) => query.OrderByDescending(p => p.Translations.Select(t => t.Name).FirstOrDefault()),
                (ProductSortBy.Price, false) => query.OrderBy(p => p.Price),
                (ProductSortBy.Price, true) => query.OrderByDescending(p => p.Price),
                (ProductSortBy.Discount, false) => query.OrderBy(p => p.Discount),
                (ProductSortBy.Discount, true) => query.OrderByDescending(p => p.Discount),
                (ProductSortBy.Quantity, false) => query.OrderBy(p => p.Quantity),
                (ProductSortBy.Quantity, true) => query.OrderByDescending(p => p.Quantity),
                (ProductSortBy.Rate, false) => query.OrderBy(p => p.Rate),
                (ProductSortBy.Rate, true) => query.OrderByDescending(p => p.Rate),
                (ProductSortBy.CreatedOn, false) => query.OrderBy(p => p.CreatedOn),
                (ProductSortBy.CreatedOn, true) => query.OrderByDescending(p => p.CreatedOn),
                (ProductSortBy.Id, false) => query.OrderBy(p => p.Id),
                _ => query.OrderByDescending(p => p.Id)
            };

            var paginatedResult = await query.ToPaginateAsync(queryRequest.PageNumber, queryRequest.PageSize);
            var response = paginatedResult.Data.Adapt<List<ProductResponse>>();

            var result = new paginationResponse<ProductResponse>
            {
                Data = response,
                TotalCount = paginatedResult.TotalCount,
                PageNumber = paginatedResult.PageNumber,
                PageSize = paginatedResult.PageSize
            };
            return Result<paginationResponse<ProductResponse>>.Ok(result);
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









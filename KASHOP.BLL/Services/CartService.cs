using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Models;
using Microsoft.AspNetCore.Identity;
using KASHOP.DAL.Dto;
using Mapster;
using System.Linq.Expressions;
using KASHOP.DAL.Repository;
using System.Globalization;
using KASHOP.BLL.Extensions;


namespace KASHOP.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }



        public async Task<Result<bool>> AddToCart(string userId, CartItemRequest cartRequest)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<bool>.Fail("User not found");
            }

            var product = await _unitOfWork.ProductRepository.GetOneAsync(p => p.Id == cartRequest.ProductId);

            if (product == null)
            {
                return Result<bool>.Fail("Product not found");
            }
            if (cartRequest.Count <= 0)
            {
                return Result<bool>.Fail("Count must be greater than zero");
            }

            if (cartRequest.Count > product.Quantity)
            {
                return Result<bool>.Fail("Count exceeds available stock");
            }
            var existingCartItem = await _unitOfWork.CartRepository.GetOneAsync(c => c.ProductId == cartRequest.ProductId && c.UserId == userId);

            if (existingCartItem != null)
            {
                existingCartItem.Count += cartRequest.Count;
            }
            else
            {
                var newCartItem = new CartItem
                {
                    ProductId = cartRequest.ProductId,
                    UserId = userId,
                    Count = cartRequest.Count
                };
                await _unitOfWork.CartRepository.CreateAsync(newCartItem);
            }
            await _unitOfWork.CompleteAsync();
            return Result<bool>.Ok(true);
        }

       

        public async Task<Result<paginationResponse<CartItemResponse>>> GetCartItemsAsync(string userId, CartQueryRequest queryRequest)
        {
            var query = _unitOfWork.CartRepository.GetQueryable(c => c.UserId == userId, [nameof(CartItem.Product), $"{nameof(CartItem.Product)}.{nameof(Product.Translations)}"]);
            if (!string.IsNullOrWhiteSpace(queryRequest.Search))
                query = query.Where(c => c.Product.Translations.Any(t => t.Name.Contains(queryRequest.Search)));

            query = (queryRequest.SortBy, queryRequest.SortDescending) switch
            {
                (CartSortBy.ProductName, false) => query.OrderBy(c => c.Product.Translations.Select(t => t.Name).FirstOrDefault()),
                (CartSortBy.ProductName, true) => query.OrderByDescending(c => c.Product.Translations.Select(t => t.Name).FirstOrDefault()),
                (CartSortBy.Price, false) => query.OrderBy(c => c.Product.Price),
                (CartSortBy.Price, true) => query.OrderByDescending(c => c.Product.Price),
                (CartSortBy.Count, false) => query.OrderBy(c => c.Count),
                (CartSortBy.Count, true) => query.OrderByDescending(c => c.Count),
                (CartSortBy.Total, false) => query.OrderBy(c => c.Count * c.Product.Price),
                _ => query.OrderByDescending(c => c.Count * c.Product.Price)
            };
            var page = await query.ToPaginateAsync(queryRequest.PageNumber, queryRequest.PageSize);
            return Result<paginationResponse<CartItemResponse>>.Ok(new paginationResponse<CartItemResponse>
            {
                Data = page.Data.Adapt<List<CartItemResponse>>(),
                TotalCount = page.TotalCount,
                PageNumber = page.PageNumber,
                PageSize = page.PageSize
            });
        }

        public async Task<Result<CartItemResponse>> GetCartItemAsync(string userId, int productId)
        {
            var cartItem = await _unitOfWork.CartRepository.GetOneAsync(
                c => c.UserId == userId && c.ProductId == productId,
                [nameof(CartItem.Product), $"{nameof(CartItem.Product)}.{nameof(Product.Translations)}"]);
            return cartItem == null
                ? Result<CartItemResponse>.Fail("Cart item not found")
                : Result<CartItemResponse>.Ok(cartItem.Adapt<CartItemResponse>());
        }

     

        public  async Task<Result<bool>> RemoveFromCart(string userId, int productId)
        {
            var cartItem = await _unitOfWork.CartRepository.GetOneAsync(c => c.UserId == userId && c.ProductId == productId);
            if (cartItem == null)
            {
                return Result<bool>.Fail("Cart item not found");
            }
            _unitOfWork.CartRepository.Delete(cartItem);
            await _unitOfWork.CompleteAsync();
            return Result<bool>.Ok(true);
        }


        public async Task<Result<bool>> ClearCart(string userId)
        {
            var cartItems = await _unitOfWork.CartRepository.GetAllAsync(c => c.UserId == userId);
            if (cartItems == null || !cartItems.Any())
            {
                return Result<bool>.Fail("Cart is already empty");
            }
            foreach (var cartItem in cartItems)
            {
                _unitOfWork.CartRepository.Delete(cartItem);
            }
            await _unitOfWork.CompleteAsync();
            return Result<bool>.Ok(true);
        }

        public  async Task<Result<bool>> UpdateQuantity(string userId, int productId, int newCount)
        {
            var cartItem = await _unitOfWork.CartRepository.GetOneAsync(c => c.UserId == userId && c.ProductId == productId);
            if (cartItem == null)
            {
                return Result<bool>.Fail("Cart item not found");
            }
            var product = await _unitOfWork.ProductRepository.GetOneAsync(p => p.Id == productId);
            if (product == null)
            {
                return Result<bool>.Fail("Product not found");
            }
            if (newCount <= 0)
            {
                return Result<bool>.Fail("Count must be greater than zero");
            }
            if (newCount > product.Quantity)
            {
                return Result<bool>.Fail("Count exceeds available stock");
            }
            cartItem.Count = newCount;
            await _unitOfWork.CompleteAsync();
            return Result<bool>.Ok(true);
        }
    }
}



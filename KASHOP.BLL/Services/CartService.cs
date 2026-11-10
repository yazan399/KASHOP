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
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        public async Task<Result<bool>> AddToCart(string userId, CartItemRequest cartRequest)
        {

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

       

        public async Task<Result<List<CartItemResponse>>> GetCartItemsAsync(string userId)
        {
            var cartItems =
            await _unitOfWork.CartRepository.GetAllAsync(c => c.UserId == userId,includes: new[] { nameof(CartItem.Product) ,$"{nameof(CartItem.Product)}.{nameof(Product.Translations)}" });

            var response = cartItems.Adapt<List<CartItemResponse>>();
            return Result<List<CartItemResponse>>.Ok(response);
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


        public Task<Result<bool>> ClearCart(string userId)
        {
            var cartItems = _unitOfWork.CartRepository.GetAllAsync(c => c.UserId == userId).Result;
            if (cartItems == null || !cartItems.Any())
            {
                return Task.FromResult(Result<bool>.Fail("Cart is already empty"));
            }
            foreach (var cartItem in cartItems)
            {
                _unitOfWork.CartRepository.Delete(cartItem);
            }
            _unitOfWork.CompleteAsync().Wait();
            return Task.FromResult(Result<bool>.Ok(true));
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



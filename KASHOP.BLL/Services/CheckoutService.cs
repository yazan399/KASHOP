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
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Headers;

using System.Globalization;

namespace KASHOP.BLL.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public CheckoutService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }




        public async Task<Result<OrderResponse>> CheckoutAsync(string userId, CheckoutRequest request)
        {

            var cartItems = await _unitOfWork.CartRepository.GetAllAsync(
                c => c.UserId == userId,
                new[]
                {
                    nameof(CartItem.Product),
                    $"{nameof(CartItem.Product)}.{nameof(Product.Translations)}"
                });
            if (cartItems == null)
            {
                return Result<OrderResponse>.Fail("User not found");
            }
            if (cartItems == null || !cartItems.Any())
            {
                return Result<OrderResponse>.Fail("Cart is empty");
            }

            foreach (var item in cartItems)
            {
                if (item.Product == null)
                {
                    return Result<OrderResponse>.Fail($" Product with Id {item.ProductId} not found");
                }
                if (item.Product.Translations == null || !item.Product.Translations.Any(t =>
                    t.Language == CultureInfo.CurrentUICulture.Name &&
                    !string.IsNullOrWhiteSpace(t.Name)))
                {
                    return Result<OrderResponse>.Fail($"Product with Id {item.ProductId} has no name for the current language");
                }
                if (item.Count > item.Product.Quantity)
                {
                    return Result<OrderResponse>.Fail($"Not enough stock for product {item.Product.Id}. Available: {item.Product.Quantity}, Requested: {item.Count}");
                }

            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<OrderResponse>.Fail("User not found");
            }

            var city = request.City ?? user.City;
            var street = request.Street ?? user.Street;
            var phoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(phoneNumber))
            {
                return Result<OrderResponse>.Fail("City, Street, and Phone Number are required.");
            }

            // Create the order and order items.... why didnt use adapt and did manually? 
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = DAL.Enums.OrderStatusEnum.Pending,
                PaymentStatus = DAL.Enums.PaymentStatusEnum.Pending,
                PaymentMethod = request.PaymentMethod,
                City = city,
                Street = street,
                PhoneNumber = phoneNumber,
                OrderItems = new List<OrderItem>() // Initialize the OrderItems list    
            };



            // Calculate the total amount and create order items
            decimal TotalAmount = 0; 

            foreach (var item in cartItems)
            {
               var orderItem = new OrderItem
               {
                   OrderId = order.Id,
                   ProductId = item.ProductId,
                   Product = item.Product,
                   Count = item.Count,
                   PriceAtPurchase = item.Product.Price
               };
               order.OrderItems.Add(orderItem);
                TotalAmount += item.Count * item.Product.Price;

               
            }

           order.TotalAmount = TotalAmount;

            await _unitOfWork.OrderRepository.CreateAsync(order);
            foreach (var item in cartItems)
            {
               _unitOfWork.CartRepository.Delete(item);
            }

            await _unitOfWork.CompleteAsync();
            var response = order.Adapt<OrderResponse>();
            return Result<OrderResponse>.Ok(response);
        }

            
        
   
    }
}

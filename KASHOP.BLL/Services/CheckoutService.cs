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

            // Create the order and his list of order items
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



            // Calculate the total amount and create order items one by one in the loop
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



            if(request.PaymentMethod == DAL.Enums.PaymentMethodEnum.Cash)
            {
                return await PlaceCashOrderAsync(order, cartItems);
            }
             else return await PlaceVisaOrderAsync(order, cartItems);







        }


        private async Task<Result<OrderResponse>> PlaceCashOrderAsync(Order order, List<CartItem> cartItems)
        {
            foreach (var item in cartItems)
            {
                item.Product.Quantity -= item.Count; // Decrease the product quantity
                _unitOfWork.CartRepository.Delete(item);
            }
            await _unitOfWork.OrderRepository.CreateAsync(order);
            await _unitOfWork.CompleteAsync();
            var response = order.Adapt<OrderResponse>();
            return Result<OrderResponse>.Ok(response, "Order placed successfully with cash payment.");
        }


        private async Task<Result<OrderResponse>> PlaceVisaOrderAsync(Order order, List<CartItem> cartItems)
        {
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = "https://localhost:44396/checkout/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://localhost:44396/checkout/cancel",
            };
            foreach (var item in cartItems)
            {
                var lineItem = new Stripe.Checkout.SessionLineItemOptions
                {
                    PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = item.Product.Price * 100, // Convert to cents
                        Currency = "usd",
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Translations.First(t => t.Language == CultureInfo.CurrentUICulture.Name).Name,

                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(lineItem);
            }
            var service = new Stripe.Checkout.SessionService();
            var session = await service.CreateAsync(options);
            order.StripeSessionId = session.Id;
            order.StripePaymentIntentId = session.PaymentIntentId;
            await _unitOfWork.OrderRepository.CreateAsync(order);
            await _unitOfWork.CompleteAsync();
            var response = order.Adapt<OrderResponse>();
            response.PaymentUrl = session.Url;
            return Result<OrderResponse>.Ok(response, "Order placed successfully. Redirect to payment.");
        }

    }
}

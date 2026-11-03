using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KASHOP.DAL.Dto;
using System.Threading.Tasks;
using KASHOP.DAL.Models;
using Microsoft.AspNetCore.Identity;
using KASHOP.DAL.Data;
using Microsoft.EntityFrameworkCore;
using KASHOP.DAL.Repository;
using KASHOP.BLL.Services;
using KASHOP.BLL.Common;
using Mapster;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace KASHOP.BLL.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public AuthenticationService(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;

        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new LoginResponse { Message = "Invalid Email." };
            }

            if(!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new LoginResponse { Message = "Email is not confirmed." };
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                return new LoginResponse { Message = "Invalid password." };
            }

            return new LoginResponse { Message = "Login successful." };
        }

        public async Task<bool> ConfirmEmail(ConfirmEmailRequest request)
        {

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return false;
            }
            request.Token = Uri.UnescapeDataString(request.Token); // Unescape the token to get the original value

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, request.Password);


            if (result.Succeeded)
            {

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                token = Uri.EscapeDataString(token); // Escape the token to make it URL-safe
                var emailUrl = $"https://localhost:7158/api/Account/ConfirmEmail?token={token}&userId={user.Id}";
                await _emailSender.SendEmailAsync(user.Email, "confirm email", $"<h1>Welcome to our app</h1><p>Please click the link to confirm your email: <a href=\"{emailUrl}\">Confirm Email</a></p>");

                return new RegisterResponse { Message = "User registered successfully." };
            }
            else
            {
                return new RegisterResponse { Message = "Failed to register user." };
            }


        }
    }
}
    
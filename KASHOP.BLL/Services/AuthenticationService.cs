using KASHOP.BLL.Common;
using KASHOP.BLL.Services;
using KASHOP.DAL.Data;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;



namespace KASHOP.BLL.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public AuthenticationService(IConfiguration config, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _config = config;
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
            var token = await GenerateJwt(user);
            return new LoginResponse { Message = "Login successful.", Token = token };
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
        private async Task<string> GenerateJwt(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, string.Join(",", roles))
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["ApiSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["ApiSettings:Issuer"],
                audience: _config["ApiSettings:Audience"],
                claims: userClaims,
                expires: DateTime.UtcNow.AddDays(20),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
    
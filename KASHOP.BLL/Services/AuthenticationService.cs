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
using KASHOP.BLL.Extensions;



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
        public async Task<Result<paginationResponse<UserResponse>>> GetAllUsersAsync(UserQueryRequest queryRequest)
        {
            var query = _userManager.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(queryRequest.Search))
                query = query.Where(u => u.UserName!.Contains(queryRequest.Search) || u.Email!.Contains(queryRequest.Search) || u.FullName.Contains(queryRequest.Search));
            if (queryRequest.EmailConfirmed.HasValue)
                query = query.Where(u => u.EmailConfirmed == queryRequest.EmailConfirmed.Value);

            query = (queryRequest.SortBy, queryRequest.SortDescending) switch
            {
                (UserSortBy.Id, false) => query.OrderBy(u => u.Id),
                (UserSortBy.Id, true) => query.OrderByDescending(u => u.Id),
                (UserSortBy.Email, false) => query.OrderBy(u => u.Email),
                (UserSortBy.Email, true) => query.OrderByDescending(u => u.Email),
                (UserSortBy.FullName, false) => query.OrderBy(u => u.FullName),
                (UserSortBy.FullName, true) => query.OrderByDescending(u => u.FullName),
                (UserSortBy.UserName, false) => query.OrderBy(u => u.UserName),
                _ => query.OrderByDescending(u => u.UserName)
            };

            var page = await query.ToPaginateAsync(queryRequest.PageNumber, queryRequest.PageSize);
            var users = new List<UserResponse>();
            foreach (var user in page.Data)
            {
                users.Add(new UserResponse
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList()
                });
            }
            return Result<paginationResponse<UserResponse>>.Ok(new paginationResponse<UserResponse>
            {
                Data = users,
                TotalCount = page.TotalCount,
                PageNumber = page.PageNumber,
                PageSize = page.PageSize
            });
        }

        public async Task<Result<UserResponse>> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<UserResponse>.Fail("User not found.");
            }

            return Result<UserResponse>.Ok(new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            });
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result<LoginResponse>.Fail("Invalid Email.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Result<LoginResponse>.Fail("Email is not confirmed.");
            }

            var passwordIsValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordIsValid)
            {
                return Result<LoginResponse>.Fail("Invalid password.");
            }

            var token = await GenerateJwt(user);
            return Result<LoginResponse>.Ok(new LoginResponse
            {
                Message = "Login successful.",
                Token = token
            });
        }

        public async Task<Result<bool>> ConfirmEmail(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<bool>.Fail("User not found.");
            }

            request.Token = Uri.UnescapeDataString(request.Token);

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded)
            {
                return Result<bool>.Fail(string.Join("; ", result.Errors.Select(error => error.Description)));
            }

            return Result<bool>.Ok(true);
        }
        

        public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return Result<RegisterResponse>.Fail(string.Join("; ", result.Errors.Select(error => error.Description)));
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!addRoleResult.Succeeded)
            {
                return Result<RegisterResponse>.Fail(string.Join("; ", addRoleResult.Errors.Select(error => error.Description)));
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = Uri.EscapeDataString(token);
            var emailUrl = $"https://localhost:7158/api/Account/ConfirmEmail?token={token}&userId={user.Id}";
            await _emailSender.SendEmailAsync(user.Email, "confirm email", $"<h1>Welcome to our app</h1><p>Please click the link to confirm your email: <a href=\"{emailUrl}\">Confirm Email</a></p>");

            return Result<RegisterResponse>.Ok(new RegisterResponse
            {
                Message = "User registered successfully."
            });
        }
        private async Task<string> GenerateJwt(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };
            userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
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


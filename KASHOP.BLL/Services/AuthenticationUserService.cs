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
using Mapster;


namespace KASHOP.BLL.Services
{
    public class AuthenticationUserService : IAuthenticationUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthenticationUserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return new RegisterResponse { Message = "User registered successfully." };
            }
            else
            {
                return new RegisterResponse { Message = "Failed to register user." };
            }

        }
    }
}
    
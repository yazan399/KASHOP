using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Models;
using KASHOP.DAL.Dto;

namespace KASHOP.BLL.Services
{
    public interface IAuthenticationService
    {
        public Task<Result<bool>> ConfirmEmail(ConfirmEmailRequest request);
        public Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
        public Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request);
        public Task<Result<List<ApplicationUser>>> GetAllUsersAsync();
    }
}   

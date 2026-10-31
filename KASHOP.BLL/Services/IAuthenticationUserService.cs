using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Dto;

namespace KASHOP.BLL.Services
{
    public interface IAuthenticationUserService
    {
        public Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    }
}   

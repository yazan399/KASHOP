using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using KASHOP.PL.Resources;
using KASHOP.DAL.Dto;
using Mapster;
using KASHOP.DAL.Models;
using KASHOP.DAL.Data;
using Microsoft.EntityFrameworkCore;
using KASHOP.DAL.Repository;
using KASHOP.BLL.Services;


namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationUserService;

        public AccountController(IAuthenticationService authenticationUserService)
        {
            _authenticationUserService = authenticationUserService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register( RegisterRequest request)
        {
            var response = await _authenticationUserService.RegisterAsync(request);
            return Ok(response);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authenticationUserService.LoginAsync(request);
            return Ok(response);
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string email)
        {
            return Ok($"Email {email} confirmed successfully.");
        }

    }
}

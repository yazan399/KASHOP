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
        [HttpGet("users")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _authenticationUserService.GetAllUsersAsync();
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register( [FromBody] RegisterRequest request)
        {
            var response = await _authenticationUserService.RegisterAsync(request);
            return Ok(response);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login( [FromBody] LoginRequest request)
        {
            var response = await _authenticationUserService.LoginAsync(request);
            return Ok(response);
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail( [FromQuery]ConfirmEmailRequest request)
        {
            var isConfirmed = await _authenticationUserService.ConfirmEmail(request);
            if (isConfirmed)
            {
                return Content($"Email confirmed successfully for user {request.UserId} with token {request.Token}");
            }
            else
            {
                return BadRequest("Failed to confirm email.");
            }
        }

    }
}

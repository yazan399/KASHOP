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
            var result = await _authenticationUserService.GetAllUsersAsync();
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = result.Message });
            }

            return Ok(result.Data);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register( [FromBody] RegisterRequest request)
        {
            var result = await _authenticationUserService.RegisterAsync(request);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(result.Data);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login( [FromBody] LoginRequest request)
        {
            var result = await _authenticationUserService.LoginAsync(request);
            if (!result.Success)
            {
                return Unauthorized(new { Message = result.Message });
            }

            return Ok(result.Data);
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail( [FromQuery]ConfirmEmailRequest request)
        {
            var result = await _authenticationUserService.ConfirmEmail(request);
            if (result.Success && result.Data == true)
            {
                return Content($"Email confirmed successfully for user {request.UserId} with token {request.Token}");
            }

            return BadRequest(result.Message);
        }

    }
}

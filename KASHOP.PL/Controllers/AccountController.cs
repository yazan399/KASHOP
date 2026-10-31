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
        private readonly IAuthenticationUserService _authenticationUserService;

        public AccountController(IAuthenticationUserService authenticationUserService)
        {
            _authenticationUserService = authenticationUserService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register( RegisterRequest request)
        {
            var response = await _authenticationUserService.RegisterAsync(request);
            return Ok(response);
        }

    }
}

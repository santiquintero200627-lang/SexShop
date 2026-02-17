using Microsoft.AspNetCore.Mvc;
using SexShop.Application.DTOs.Auth;
using SexShop.Application.Interfaces;

namespace SexShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("add-role")]
        // [Authorize(Roles = "Admin")] // Uncomment to secure
        public async Task<IActionResult> AddRole(string email, string role)
        {
            var result = await _authService.AddRoleAsync(email, role);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}

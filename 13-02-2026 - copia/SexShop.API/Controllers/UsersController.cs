using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SexShop.Application.Interfaces;

namespace SexShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _authService.GetAllUsersAsync();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _authService.DeleteUserAsync(id);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}

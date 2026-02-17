using SexShop.Application.Common;
using SexShop.Application.DTOs.Auth;

namespace SexShop.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<string>> AddRoleAsync(string email, string roleName); // For Admin
        Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync();
        Task<ApiResponse<string>> DeleteUserAsync(string userId);
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SexShop.Application.Common;
using SexShop.Application.DTOs.Auth;
using SexShop.Application.Interfaces;
using SexShop.Application.Settings;
using SexShop.Domain.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SexShop.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return new ApiResponse<AuthResponseDto>("Invalid email or password.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return new ApiResponse<AuthResponseDto>("Invalid email or password.");

            var token = await GenerateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            return new ApiResponse<AuthResponseDto>(new AuthResponseDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList(),
                Token = token,
                RefreshToken = "" // Implement if needed
            }, "Login Successful");
        }

        public async Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
                return new ApiResponse<string>("User already exists.");

            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return new ApiResponse<string>(result.Errors.Select(e => e.Description).ToList(), "Registration Failed");

            // Default role: Guest logic or explicit assignment. 
            // We will assign 'Guest' if not Admin.
            // Ensure roles exist first (seeded).
            await _userManager.AddToRoleAsync(user, "Guest");

            return new ApiResponse<string>(user.Id, "User registered successfully.");
        }

        public async Task<ApiResponse<string>> AddRoleAsync(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return new ApiResponse<string>("User not found.");

            if (!await _roleManager.RoleExistsAsync(roleName))
                return new ApiResponse<string>("Role does not exist.");

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
                return new ApiResponse<string>("Failed to add role.");

            return new ApiResponse<string>($"Role {roleName} added to user.");
        }

        public async Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList()
                });
            }

            return new ApiResponse<IEnumerable<UserDto>>(userDtos);
        }

        public async Task<ApiResponse<string>> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new ApiResponse<string>("User not found.");

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return new ApiResponse<string>("Cannot delete an Admin user.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return new ApiResponse<string>("Failed to delete user.");

            return new ApiResponse<string>(userId, "User deleted successfully.");
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim("uid", user.Id)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

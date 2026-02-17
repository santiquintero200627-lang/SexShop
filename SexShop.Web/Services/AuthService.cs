using SexShop.Application.Common;
using SexShop.Application.DTOs.Auth;
using SexShop.Application.Interfaces;
using System.Net.Http.Json;

namespace SexShop.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("SexShopAPI");
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddAuthorization(HttpRequestMessage request)
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("Token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", loginDto);
            if (!response.IsSuccessStatusCode)
                return new ApiResponse<AuthResponseDto>("Correo o contraseña inválidos.");

            return await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>() 
                   ?? new ApiResponse<AuthResponseDto>("Error de comunicación con el API");
        }

        public async Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto)
        {
             var response = await _httpClient.PostAsJsonAsync("auth/register", registerDto);
             if (!response.IsSuccessStatusCode)
                 return new ApiResponse<string>("Error en el registro: " + response.StatusCode);

             return await response.Content.ReadFromJsonAsync<ApiResponse<string>>()
                    ?? new ApiResponse<string>("Error de comunicación con el API");
        }

        public async Task<ApiResponse<string>> AddRoleAsync(string email, string roleName)
        {
             using var request = new HttpRequestMessage(HttpMethod.Post, $"auth/add-role?email={email}&roleName={roleName}");
             AddAuthorization(request);

             var response = await _httpClient.SendAsync(request);
             return await response.Content.ReadFromJsonAsync<ApiResponse<string>>()
                    ?? new ApiResponse<string>("Error al asignar rol");
        }

        public async Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "users");
            AddAuthorization(request);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return new ApiResponse<IEnumerable<UserDto>>("Error: " + response.StatusCode);

            return await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<UserDto>>>()
                   ?? new ApiResponse<IEnumerable<UserDto>>("Error al obtener usuarios");
        }

        public async Task<ApiResponse<string>> DeleteUserAsync(string userId)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"users/{userId}");
            AddAuthorization(request);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                 return new ApiResponse<string>("Error: " + response.StatusCode);

            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>()
                   ?? new ApiResponse<string>("Error al eliminar usuario");
        }
    }
}

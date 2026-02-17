using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SexShop.Application.DTOs.Auth;
using SexShop.Application.Interfaces;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SexShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid) return View(loginDto);

            var response = await _authService.LoginAsync(loginDto);
            if (response.Succeeded && response.Data != null)
            {
                var token = response.Data.Token;
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var claims = new List<Claim>(jwtToken.Claims);
                claims.Add(new Claim("Token", token)); // Store token for API calls

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = jwtToken.ValidTo
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View(loginDto);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid) return View(registerDto);

            var response = await _authService.RegisterAsync(registerDto);
            if (response.Succeeded)
            {
                return RedirectToAction("Login");
            }

            if (response.Errors != null)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
            }

            return View(registerDto);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

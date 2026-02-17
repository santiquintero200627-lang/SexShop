using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SexShop.Application.Interfaces;
using SexShop.Application.DTOs.Products;
using SexShop.Application.DTOs.Auth;

namespace SexShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly IAuthService _authService;

        public AdminController(IProductService productService, IAuthService authService)
        {
            _productService = productService;
            _authService = authService;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> Products()
        {
            var result = await _productService.GetAllProductsAsync();
            return View(result.Data);
        }

        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                ModelState.AddModelError("", "Validación local falló: " + errors);
                return View(createProductDto);
            }

            try
            {
                var result = await _productService.CreateProductAsync(createProductDto);
                if (result.Succeeded) return RedirectToAction(nameof(Products));

                ModelState.AddModelError("", "Error del API: " + result.Message);
                if (result.Errors != null && result.Errors.Any())
                {
                    foreach (var err in result.Errors) ModelState.AddModelError("", err);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Excepción crítica: " + ex.Message);
            }

            return View(createProductDto);
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (!result.Succeeded || result.Data == null)
            {
                return NotFound();
            }

            var product = result.Data;
            var updateDto = new UpdateProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive
            };

            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                return View(updateProductDto);
            }

            try
            {
                var result = await _productService.UpdateProductAsync(updateProductDto);
                if (result.Succeeded) return RedirectToAction(nameof(Products));

                ModelState.AddModelError("", "Error: " + result.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Excepción: " + ex.Message);
            }

            return View(updateProductDto);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Products));
        }

        public async Task<IActionResult> Users()
        {
            var result = await _authService.GetAllUsersAsync();
            return View(result.Data);
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                ModelState.AddModelError("", "Validación local falló: " + errors);
                return View(registerDto);
            }

            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                if (result.Succeeded) return RedirectToAction(nameof(Users));

                ModelState.AddModelError("", "Error del API: " + result.Message);
                if (result.Errors != null && result.Errors.Any())
                {
                    foreach (var err in result.Errors) ModelState.AddModelError("", err);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Excepción crítica: " + ex.Message);
            }

            return View(registerDto);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _authService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Users));
        }
    }
}

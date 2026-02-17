using Microsoft.AspNetCore.Mvc;
using SexShop.Application.Interfaces;

namespace SexShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _productService.GetAllProductsAsync();
            if (response.Succeeded)
            {
                return View(response.Data);
            }
            return View(new List<SexShop.Application.DTOs.Products.ProductDto>()); // Empty list on error
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}

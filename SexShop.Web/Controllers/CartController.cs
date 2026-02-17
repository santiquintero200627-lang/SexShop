using Microsoft.AspNetCore.Mvc;
using SexShop.Application.DTOs.Products;
using SexShop.Application.Interfaces;
using SexShop.Web.Extensions;
using SexShop.Web.Models;

namespace SexShop.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private const string CartSessionKey = "SexShopCart";

        public CartController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>(CartSessionKey) ?? new List<CartItemViewModel>();
            return View(cart);
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>(CartSessionKey) ?? new List<CartItemViewModel>();
            return Json(new { count = cart.Sum(i => i.Quantity) });
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var productResult = await _productService.GetProductByIdAsync(productId);
            if (!productResult.Succeeded) return NotFound();

            var product = productResult.Data!;
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>(CartSessionKey) ?? new List<CartItemViewModel>();

            var existingItem = cart.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItemViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Quantity = 1
                });
            }

            HttpContext.Session.SetObjectAsJson(CartSessionKey, cart);
            return Json(new { success = true, itemCount = cart.Sum(i => i.Quantity) });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int delta)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>(CartSessionKey) ?? new List<CartItemViewModel>();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            
            if (item != null)
            {
                item.Quantity += delta;
                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }
                
                HttpContext.Session.SetObjectAsJson(CartSessionKey, cart);
            }

            var totalItems = cart.Sum(i => i.Quantity);
            var cartTotal = cart.Sum(i => i.Total);
            var itemTotal = item?.Total ?? 0;
            var itemQuantity = item?.Quantity ?? 0;

            return Json(new 
            { 
                success = true, 
                itemRemoved = itemQuantity <= 0,
                itemQuantity = itemQuantity,
                itemTotal = itemTotal.ToString("F2"),
                cartTotal = cartTotal.ToString("F2"),
                totalItems = totalItems
            });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>(CartSessionKey) ?? new List<CartItemViewModel>();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObjectAsJson(CartSessionKey, cart);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

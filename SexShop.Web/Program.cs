using Microsoft.AspNetCore.Authentication.Cookies;
using SexShop.Web.Services;
using SexShop.Web.Services.Interfaces;
using SexShop.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURACIÓN PARA RENDER (PUERTO DINÁMICO) ---
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configuración dinámica del HttpClient
builder.Services.AddHttpClient("SexShopAPI", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] 
                  ?? "http://localhost:5005"; // Fallback local
    
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<IAuthService, AuthService>(); 
builder.Services.AddScoped<IProductService, ProductService>(); 
builder.Services.AddScoped<IOrderService, OrderService>(); 

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Render maneja el HTTPS externamente
// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
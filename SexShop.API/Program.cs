using SexShop.Application;
using SexShop.Infrastructure;
using SexShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURACIÓN PARA RENDER ---
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Policy Dinámica para Producción
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWeb",
        policy =>
        {
            var allowedOriginsStr = builder.Configuration["AllowedOrigins"];
            var allowedOrigins = !string.IsNullOrEmpty(allowedOriginsStr) 
                ? allowedOriginsStr.Split(',', StringSplitOptions.RemoveEmptyEntries)
                : new[] { "http://localhost:5185", "https://localhost:7137" };
            
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// En Render, Swagger suele ser útil incluso en "Production" para pruebas iniciales
app.UseSwagger();
app.UseSwaggerUI();

// Importante: En Render no es estrictamente necesario UseHttpsRedirection 
// porque Render maneja el SSL (HTTPS) a nivel de Proxy.
// app.UseHttpsRedirection();

app.UseRouting();

// Activar CORS antes de Auth
app.UseCors("AllowWeb");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed Data & Migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Esto asegura que la DB se cree en el disco de Render al iniciar
        await context.Database.MigrateAsync(); 
        await SexShop.API.Helpers.SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();
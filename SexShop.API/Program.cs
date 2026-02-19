using SexShop.Application;
using SexShop.Infrastructure;
using SexShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql; // <--- Nueva referencia

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

// CORS Policy Dinámica
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

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseCors("AllowWeb");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed Data & Migrations (Actualizado para PostgreSQL)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Esto aplicará las nuevas migraciones de PostgreSQL automáticamente al subir a Render
        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
        }
        
        await SexShop.API.Helpers.SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al migrar o sembrar la base de datos PostgreSQL.");
    }
}

app.Run();
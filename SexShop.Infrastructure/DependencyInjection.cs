using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SexShop.Application.Interfaces;
using SexShop.Application.Settings;
using SexShop.Domain.Entities.Identity;
using SexShop.Infrastructure.Identity;
using SexShop.Infrastructure.Persistence;
using SexShop.Infrastructure.Repositories;
using System.Text;

namespace SexShop.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Obtenemos la cadena de conexión
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada.");
            }

            // 2. Lógica de limpieza y conversión para Render/PostgreSQL
            if (connectionString.StartsWith("postgres://") || connectionString.StartsWith("postgresql://"))
            {
                // Convertimos el formato URI a formato Key=Value que prefiere Npgsql
                var uri = new Uri(connectionString);
                var userInfo = uri.UserInfo.Split(':');
                
                var username = Uri.UnescapeDataString(userInfo[0]);
                var password = Uri.UnescapeDataString(userInfo[1]);
                var host = uri.Host;
                var port = uri.Port;
                var database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/'));

                connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SslMode=Require;Trust Server Certificate=true;";
            }
            else if (!connectionString.Contains("SslMode", StringComparison.OrdinalIgnoreCase))
            {
                // Si es formato estándar pero no tiene SSL, se lo forzamos
                connectionString = connectionString.TrimEnd(';') + ";SslMode=Require;Trust Server Certificate=true;";
            }

            // 3. Configuración del DbContext con reintentos
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                          .EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

            // Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // UnitOfWork & Repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Auth Service
            services.AddScoped<IAuthService, AuthService>();

            // JWT Configuration
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!))
                };
            });

            return services;
        }
    }
}
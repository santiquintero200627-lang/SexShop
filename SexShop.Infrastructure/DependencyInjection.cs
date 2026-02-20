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
using Npgsql;

namespace SexShop.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Obtenemos la cadena de conexión (Asegúrate de que en Render sea la INTERNAL URL de Ohio)
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada.");
            }

            // 2. Usamos el Builder oficial para forzar SSL y desactivar el Pooling (Solución definitiva al EndOfStream)
            var builder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
                Pooling = false, // Evita errores de reuso de conexiones en el plan gratuito de Render
                KeepAlive = 30
            };

            // 3. Configuración del DbContext apuntando a la infraestructura de Ohio
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    builder.ConnectionString,
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
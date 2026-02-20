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
            // DbContext configurado para PostgreSQL con SSL y Reintentos para Render
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Si es una URL de PostgreSQL (común en Render), la convertimos al formato de Npgsql
            if (connectionString != null && connectionString.StartsWith("postgres://"))
            {
                var uri = new Uri(connectionString);
                var userInfo = uri.UserInfo.Split(':');
                connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SslMode=Require;Trust Server Certificate=true";
            }
            else if (connectionString != null && !connectionString.Contains("SslMode"))
            {
                // Si es una cadena normal pero no tiene SSL, se lo agregamos para Render
                connectionString += ";SslMode=Require;Trust Server Certificate=true";
            }

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
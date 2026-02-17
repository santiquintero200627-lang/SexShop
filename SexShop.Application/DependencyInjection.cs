using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SexShop.Application.Services;
using SexShop.Application.Interfaces;
using System.Reflection;

namespace SexShop.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}

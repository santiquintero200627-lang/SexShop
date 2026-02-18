using Microsoft.AspNetCore.Identity;
using SexShop.Domain.Entities;
using SexShop.Domain.Entities.Identity;
using SexShop.Infrastructure.Persistence;

namespace SexShop.API.Helpers
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new ApplicationRole { Name = "Admin", Description = "Administrator" });

            if (!await roleManager.RoleExistsAsync("Guest"))
                await roleManager.CreateAsync(new ApplicationRole { Name = "Guest", Description = "Client" });

            // Seed Admin User
            if (await userManager.FindByEmailAsync("admin@sexshop.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@sexshop.com",
                    Email = "admin@sexshop.com",
                    FirstName = "Admin",
                    LastName = "System",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            // Seed Products
            var productsToSeed = new List<Product>
            {
                new Product { Name = "Vibrador Clásico", Description = "Potente vibrador clásico de 7 velocidades.", Price = 29.99m, Stock = 50, ImageUrl = "https://th.bing.com/th/id/OIP.51EnCi1o637boc2KR-qSqgHaHa?w=204&h=204&c=7&r=0&o=7&pid=1.7&rm=3", IsActive = true },
                new Product { Name = "Lencería Sexy Roja", Description = "Conjunto de lencería de encaje rojo.", Price = 45.00m, Stock = 20, ImageUrl = "https://th.bing.com/th/id/OIP.1blPvdrBZ9SA6Kymj6VfSwHaJ3?w=157&h=208&c=7&r=0&o=7&pid=1.7&rm=3", IsActive = true },
                new Product { Name = "Aceite de Masaje", Description = "Aceite aromático para masajes relajantes.", Price = 15.50m, Stock = 100, ImageUrl = "https://th.bing.com/th/id/OIP.7EjpPOoDZ5J-6nYnSqM2WAHaHa?w=165&h=180&c=7&r=0&o=7&pid=1.7&rm=3", IsActive = true },
                new Product { Name = "Vibrador Rabbit Deluxe", Description = "Vibrador con doble estimulación, 10 modos de vibración y material siliconado premium.", Price = 49.99m, Stock = 15, ImageUrl = "https://th.bing.com/th/id/OIP.tuC5sN_Bhntrqt7Q8gYtnQHaJ5?w=160&h=213&c=7&r=0&o=7&pid=1.7&rm=3", IsActive = true },
                new Product { Name = "Plug Anal Silicona Pro", Description = "Plug anal ergonómico de silicona médica con base de seguridad.", Price = 24.50m, Stock = 25, ImageUrl = "https://th.bing.com/th/id/OIP.IIT6OPp0zvmdWLI9_dhUWAHaHa?w=188&h=188&c=7&r=0&o=7&pid=1.7&rm=3", IsActive = true },
                new Product { Name = "Esposas Ajustables BDSM", Description = "Esposas acolchadas ajustables con cierre seguro y diseño elegante.", Price = 19.99m, Stock = 30, ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR6y3vIrvcw-xPFrnIDxZ-6Q8jrN4OCz89tUw&s", IsActive = true },
                new Product { Name = "Lubricante Base Agua 250ml", Description = "Lubricante íntimo base agua, compatible con preservativos y juguetes.", Price = 12.90m, Stock = 40, ImageUrl = "https://th.bing.com/th/id/OIP.m_HSEKoXcWkR0CsYSmUlggHaHa?w=209&h=209&c=7&r=0&o=7&pid=1.7&rm=3", IsActive = true },
                new Product { Name = "Bala Vibradora Mini", Description = "Bala vibradora discreta con control remoto inalámbrico.", Price = 29.99m, Stock = 20, ImageUrl = "https://th.bing.com/th/id/OIP.tebgBDTwYoLCfh1nT4pS0QHaFj?w=261&h=196&c=7&r=0&o=7&pid=1.7&rm=3", IsActive = true },
                new Product { Name = "Kit BDSM Principiantes", Description = "Kit completo con antifaz, esposas, látigo suave y collar ajustable.", Price = 59.90m, Stock = 10, ImageUrl = "https://th.bing.com/th/id/OIP.AdWCJyrRimPP4g27sJBpxAHaHa?w=179&h=180&c=7&r=0&o=7&pid=1.7&rm=3", IsActive = true },
                new Product { Name = "Anillo Vibrador Recargable", Description = "Anillo para parejas con vibración potente y batería recargable USB.", Price = 34.99m, Stock = 18, ImageUrl = "https://th.bing.com/th/id/OIP.-ghWUfn-iAjFm85pTAglawHaFY?w=250&h=182&c=7&r=0&o=7&pid=1.7&rm=3", IsActive = true },
                new Product { Name = "Masturbador Masculino Soft Touch", Description = "Masturbador con textura interna realista y material ultra suave.", Price = 39.50m, Stock = 22, ImageUrl = "https://th.bing.com/th/id/OIP.zGX8Fov68PiCQ8fFKIgwJwHaHa?w=211&h=196&c=7&r=0&o=7&pid=1.7&rm=3", IsActive = true },
                new Product { Name = "Lencería Negra Transparente", Description = "Conjunto sensual de encaje negro con detalles elegantes.", Price = 44.99m, Stock = 12, ImageUrl = "https://www.corse.mx/cdn/shop/products/conjunto-de-lenceria-femenina-sexy-ropa_interior-pijama-seda-encaje_3.jpg?v=1749152487", IsActive = true },
                new Product { Name = "Gel Estimulante Femenino", Description = "Gel íntimo con efecto calor para mayor sensibilidad.", Price = 17.80m, Stock = 35, ImageUrl = "https://locatelcolombia.vtexassets.com/arquivos/ids/328764/7501159033749_1_Durex-Play-Gel-Lubricante-Cereza-De-Pasion-X-50Ml.jpg?v=638052509350700000", IsActive = true },
                new Product { Name = "Plug Anal Metálico Premium", Description = "Plug metálico de acero inoxidable con acabado brillante.", Price = 42.00m, Stock = 14, ImageUrl = "https://th.bing.com/th/id/OIP.h89zQZM5fD8lV-UYDs-1zwHaHa?w=215&h=215&c=7&r=0&o=7&pid=1.7&rm=3", IsActive = true },
                new Product { Name = "Vibrador Punto G Curvo", Description = "Vibrador curvo especialmente diseñado para estimulación del punto G.", Price = 36.75m, Stock = 16, ImageUrl = "https://th.bing.com/th/id/OIP.zInaL7rKiVzEmdSoroI5DwHaHa?w=218&h=218&c=7&r=0&o=7&pid=1.7&rm=3", IsActive = true }
            };

            foreach (var product in productsToSeed)
            {
                if (!context.Products.Any(p => p.Name == product.Name))
                {
                    context.Products.Add(product);
                }
            }

            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync();
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SexShop.Domain.Entities;

namespace SexShop.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            
            builder.HasMany(o => o.OrderDetails)
                   .WithOne(od => od.Order)
                   .HasForeignKey(od => od.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

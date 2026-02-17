using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SexShop.Domain.Entities;

namespace SexShop.Infrastructure.Persistence.Configurations
{
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.Property(od => od.UnitPrice).HasColumnType("decimal(18,2)");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoboCoffee.Domain.Orders;

namespace RoboCoffee.Infrastructure.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id).ValueGeneratedNever();
        builder.Property(item => item.OrderId).IsRequired();
        builder.Property(item => item.ProductId).IsRequired();
        builder.Property(item => item.ProductNameSnapshot).HasMaxLength(200).IsRequired();
        builder.Property(item => item.UnitPriceSnapshot).HasPrecision(18, 2).IsRequired();
        builder.Property(item => item.Quantity).IsRequired();
        builder.Property(item => item.Subtotal).HasPrecision(18, 2).IsRequired();
        builder.Property(item => item.Observation).HasMaxLength(500);

        builder.HasIndex(item => item.OrderId);
        builder.HasIndex(item => item.ProductId);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_order_items_quantity", "\"Quantity\" > 0");
            table.HasCheckConstraint("CK_order_items_unit_price", "\"UnitPriceSnapshot\" > 0");
            table.HasCheckConstraint("CK_order_items_subtotal", "\"Subtotal\" >= 0");
        });
    }
}

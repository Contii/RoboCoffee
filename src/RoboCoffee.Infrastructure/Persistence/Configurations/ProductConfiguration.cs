using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoboCoffee.Domain.Catalog;

namespace RoboCoffee.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(product => product.Id);

        builder.Property(product => product.Id).ValueGeneratedNever();
        builder.Property(product => product.Name).HasMaxLength(200).IsRequired();
        builder.Property(product => product.Category).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(product => product.Size).HasConversion<string>().HasMaxLength(10).IsRequired();
        builder.Property(product => product.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(product => product.IsAvailable).IsRequired();
        builder.Property(product => product.StockQuantity);
        builder.Property(product => product.IsActive).IsRequired();
        builder.Property(product => product.CreatedAtUtc).IsRequired();
        builder.Property(product => product.UpdatedAtUtc).IsRequired();
        builder.Property(product => product.RowVersion).IsRowVersion();

        builder.HasIndex(product => product.Category);
        builder.HasIndex(product => product.IsAvailable);
        builder.HasIndex(product => new { product.IsActive, product.IsAvailable });

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_products_price", "\"Price\" > 0");
            table.HasCheckConstraint("CK_products_stock", "\"StockQuantity\" IS NULL OR \"StockQuantity\" >= 0");
        });
    }
}

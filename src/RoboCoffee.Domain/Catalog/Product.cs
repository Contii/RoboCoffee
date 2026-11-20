using RoboCoffee.Domain.Exceptions;

namespace RoboCoffee.Domain.Catalog;

public sealed class Product
{
    private Product()
    {
    }

    public Product(
        Guid id,
        string name,
        ProductCategory category,
        ProductSize size,
        decimal price,
        bool isAvailable = true,
        int? stockQuantity = null)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Product id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Product name cannot be empty.");
        }

        if (price <= 0)
        {
            throw new DomainException("Product price must be greater than zero.");
        }

        if (stockQuantity is < 0)
        {
            throw new DomainException("Stock quantity cannot be negative.");
        }

        Id = id;
        Name = name.Trim();
        Category = category;
        Size = size;
        Price = price;
        IsAvailable = isAvailable;
        StockQuantity = stockQuantity;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public Guid Id { get; private init; }
    public string Name { get; private set; } = string.Empty;
    public ProductCategory Category { get; private set; }
    public ProductSize Size { get; private set; }
    public decimal Price { get; private set; }
    public bool IsAvailable { get; private set; }
    public int? StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private init; }
    public DateTime UpdatedAtUtc { get; private set; }
    public byte[]? RowVersion { get; private set; }

    public void UpdatePrice(decimal price)
    {
        if (!IsActive)
        {
            throw new DomainException("Inactive products cannot change price.");
        }

        if (price <= 0)
        {
            throw new DomainException("Product price must be greater than zero.");
        }

        Price = price;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetAvailability(bool isAvailable)
    {
        if (!IsActive)
        {
            throw new DomainException("Inactive products cannot be changed.");
        }

        IsAvailable = isAvailable;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

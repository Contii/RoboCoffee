using RoboCoffee.Domain.Catalog;
using RoboCoffee.Domain.Exceptions;

namespace RoboCoffee.Domain.Orders;

public sealed class OrderItem
{
    private OrderItem()
    {
    }

    public OrderItem(
        Guid id,
        Guid orderId,
        Guid productId,
        string productNameSnapshot,
        decimal unitPriceSnapshot,
        int quantity,
        string? observation = null)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Order item id cannot be empty.");
        }

        if (orderId == Guid.Empty)
        {
            throw new DomainException("Order id cannot be empty.");
        }

        if (productId == Guid.Empty)
        {
            throw new DomainException("Product id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(productNameSnapshot))
        {
            throw new DomainException("Product name snapshot cannot be empty.");
        }

        if (unitPriceSnapshot <= 0)
        {
            throw new DomainException("Unit price snapshot must be greater than zero.");
        }

        if (quantity <= 0)
        {
            throw new DomainException("Order item quantity must be greater than zero.");
        }

        Id = id;
        OrderId = orderId;
        ProductId = productId;
        ProductNameSnapshot = productNameSnapshot.Trim();
        UnitPriceSnapshot = unitPriceSnapshot;
        Quantity = quantity;
        Subtotal = unitPriceSnapshot * quantity;
        Observation = observation?.Trim();
    }

    public Guid Id { get; private init; }
    public Guid OrderId { get; private init; }
    public Guid ProductId { get; private init; }
    public string ProductNameSnapshot { get; private init; } = string.Empty;
    public decimal UnitPriceSnapshot { get; private init; }
    public int Quantity { get; private init; }
    public decimal Subtotal { get; private init; }
    public string? Observation { get; private init; }

    public static OrderItem Create(
        Guid orderId,
        Product product,
        int quantity,
        string? observation = null)
    {
        if (!product.IsActive)
        {
            throw new DomainException("Inactive products cannot be ordered.");
        }

        if (!product.IsAvailable)
        {
            throw new DomainException("Unavailable products cannot be ordered.");
        }

        return new OrderItem(
            Guid.NewGuid(),
            orderId,
            product.Id,
            product.Name,
            product.Price,
            quantity,
            observation);
    }
}

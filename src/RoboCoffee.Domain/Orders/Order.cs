using RoboCoffee.Domain.Catalog;
using RoboCoffee.Domain.Exceptions;

namespace RoboCoffee.Domain.Orders;

public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    private Order()
    {
    }

    private Order(
        Guid id,
        string customerName,
        string? customerDocument,
        string? postalCode)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Order id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new DomainException("Customer name cannot be empty.");
        }

        Id = id;
        CustomerName = customerName.Trim();
        CustomerDocument = customerDocument?.Trim();
        PostalCode = postalCode?.Trim();
        Status = OrderStatus.Received;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public Guid Id { get; private init; }
    public string CustomerName { get; private init; } = string.Empty;
    public string? CustomerDocument { get; private init; }
    public string? PostalCode { get; private init; }
    public OrderStatus Status { get; private set; }
    public decimal Total { get; private set; }
    public DateTime CreatedAtUtc { get; private init; }
    public DateTime UpdatedAtUtc { get; private set; }
    public byte[]? RowVersion { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public static Order Create(
        Guid id,
        string customerName,
        string? customerDocument,
        string? postalCode)
    {
        return new Order(id, customerName, customerDocument, postalCode);
    }

    public void AddItem(Product product, int quantity, string? observation = null)
    {
        if (Status != OrderStatus.Received)
        {
            throw new DomainException("Items can only be added while the order is received.");
        }

        var item = OrderItem.Create(Id, product, quantity, observation);
        _items.Add(item);
        CalculateTotal();
    }

    public void CalculateTotal()
    {
        Total = _items.Sum(item => item.Subtotal);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void EnsureHasItems()
    {
        if (_items.Count == 0)
        {
            throw new DomainException("Order must contain at least one item.");
        }
    }

    public void AdvanceStatus()
    {
        Status = Status switch
        {
            OrderStatus.Received => OrderStatus.Preparing,
            OrderStatus.Preparing => OrderStatus.Ready,
            OrderStatus.Ready => OrderStatus.Delivered,
            OrderStatus.Delivered => throw new DomainException("Delivered orders cannot advance."),
            OrderStatus.Cancelled => throw new DomainException("Cancelled orders cannot advance."),
            _ => throw new DomainException("Unknown order status.")
        };

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Received)
        {
            throw new DomainException("Only received orders can be cancelled.");
        }

        Status = OrderStatus.Cancelled;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

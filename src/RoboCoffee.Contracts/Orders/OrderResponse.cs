namespace RoboCoffee.Contracts.Orders;

public sealed record OrderResponse(
    Guid Id,
    string CustomerName,
    string? CustomerDocument,
    string? PostalCode,
    string Status,
    decimal Total,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    IReadOnlyList<OrderItemResponse> Items);

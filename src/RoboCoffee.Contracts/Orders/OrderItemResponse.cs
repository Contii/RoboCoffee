namespace RoboCoffee.Contracts.Orders;

public sealed record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductNameSnapshot,
    int Quantity,
    decimal UnitPriceSnapshot,
    decimal Subtotal,
    string? Observation);

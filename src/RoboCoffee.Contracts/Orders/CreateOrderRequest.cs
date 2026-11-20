namespace RoboCoffee.Contracts.Orders;

public sealed record CreateOrderRequest(
    string CustomerName,
    string? CustomerDocument,
    string? PostalCode,
    IReadOnlyList<CreateOrderItemRequest> Items);

namespace RoboCoffee.Contracts.Catalog;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Category,
    string Size,
    decimal Price,
    bool IsAvailable,
    int? StockQuantity,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

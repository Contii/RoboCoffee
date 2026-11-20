namespace RoboCoffee.Contracts.Catalog;

public sealed record UpdateProductRequest(
    string Name,
    string Category,
    string Size,
    decimal Price,
    bool IsAvailable,
    int? StockQuantity);

namespace RoboCoffee.Contracts.Catalog;

public sealed record CreateProductRequest(
    string Name,
    string Category,
    string Size,
    decimal Price,
    bool IsAvailable,
    int? StockQuantity);

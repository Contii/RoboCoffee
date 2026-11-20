namespace RoboCoffee.Application.Catalog;

public sealed class ProductNotFoundException : Exception
{
    public ProductNotFoundException(Guid productId)
        : base($"Product '{productId}' was not found.")
    {
    }
}

using Microsoft.EntityFrameworkCore;
using RoboCoffee.Application.Catalog;
using RoboCoffee.Contracts.Catalog;
using RoboCoffee.Domain.Catalog;
using RoboCoffee.Domain.Exceptions;
using RoboCoffee.Infrastructure.Persistence;

namespace RoboCoffee.Infrastructure.Catalog;

public sealed class ProductCatalogService : IProductCatalogService
{
    private readonly RoboCoffeeDbContext _context;

    public ProductCatalogService(RoboCoffeeDbContext context)
    {
        _context = context;
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = new Product(
            Guid.NewGuid(),
            request.Name,
            ParseCategory(request.Category),
            ParseSize(request.Size),
            request.Price,
            request.IsAvailable,
            request.StockQuantity);

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        return Map(product);
    }

    public async Task<PagedResult<ProductResponse>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than zero.");
        }

        if (pageSize is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be between 1 and 100.");
        }

        var query = _context.Products.Where(product => product.IsActive);
        var totalItems = await query.CountAsync(cancellationToken);
        var products = await query
            .OrderBy(product => product.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductResponse>(
            products.Select(Map).ToList(),
            page,
            pageSize,
            totalItems);
    }

    public async Task<ProductResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate =>
                candidate.Id == id && candidate.IsActive,
                cancellationToken);

        return product is null
            ? throw new ProductNotFoundException(id)
            : Map(product);
    }

    public async Task<ProductResponse> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(candidate =>
                candidate.Id == id && candidate.IsActive,
                cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(id);
        }

        product.UpdateDetails(
            request.Name,
            ParseCategory(request.Category),
            ParseSize(request.Size),
            request.Price,
            request.IsAvailable,
            request.StockQuantity);

        await _context.SaveChangesAsync(cancellationToken);
        return Map(product);
    }

    public async Task DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(candidate =>
                candidate.Id == id && candidate.IsActive,
                cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(id);
        }

        product.Deactivate();
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static ProductCategory ParseCategory(string category)
    {
        if (!Enum.TryParse<ProductCategory>(category, true, out var result))
        {
            throw new DomainException($"Unknown product category '{category}'.");
        }

        return result;
    }

    private static ProductSize ParseSize(string size)
    {
        if (!Enum.TryParse<ProductSize>(size, true, out var result))
        {
            throw new DomainException($"Unknown product size '{size}'.");
        }

        return result;
    }

    private static ProductResponse Map(Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Category.ToString(),
            product.Size.ToString(),
            product.Price,
            product.IsAvailable,
            product.StockQuantity,
            product.IsActive,
            product.CreatedAtUtc,
            product.UpdatedAtUtc);
    }
}

using Microsoft.EntityFrameworkCore;
using RoboCoffee.Application.Catalog;
using RoboCoffee.Application.Orders;
using RoboCoffee.Contracts.Catalog;
using RoboCoffee.Contracts.Orders;
using RoboCoffee.Domain.Exceptions;
using RoboCoffee.Domain.Orders;
using RoboCoffee.Infrastructure.Persistence;

namespace RoboCoffee.Infrastructure.Orders;

public sealed class OrderService : IOrderService
{
    private readonly RoboCoffeeDbContext _context;

    public OrderService(RoboCoffeeDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<OrderResponse>> GetPagedAsync(
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

        var query = _context.Orders
            .AsNoTracking()
            .Include(order => order.Items);

        var totalItems = await query.CountAsync(cancellationToken);
        var orders = await query
            .OrderByDescending(order => order.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<OrderResponse>(
            orders.Select(Map).ToList(),
            page,
            pageSize,
            totalItems);
    }

    public async Task<OrderResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(candidate => candidate.Items)
            .FirstOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        return order is null
            ? throw new OrderNotFoundException(id)
            : Map(order);
    }

    public async Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Items is null || request.Items.Count == 0)
        {
            throw new DomainException("Order must contain at least one item.");
        }

        var productIds = request.Items
            .Select(item => item.ProductId)
            .Distinct()
            .ToArray();

        await using var transaction = _context.Database.IsRelational()
            ? await _context.Database.BeginTransactionAsync(cancellationToken)
            : null;

        var products = await _context.Products
            .Where(product => productIds.Contains(product.Id) && product.IsActive)
            .ToDictionaryAsync(product => product.Id, cancellationToken);

        foreach (var productId in productIds)
        {
            if (!products.ContainsKey(productId))
            {
                throw new ProductNotFoundException(productId);
            }
        }

        var order = Order.Create(
            Guid.NewGuid(),
            request.CustomerName,
            request.CustomerDocument,
            request.PostalCode);

        foreach (var item in request.Items)
        {
            order.AddItem(products[item.ProductId], item.Quantity, item.Observation);
        }

        order.EnsureHasItems();
        order.CalculateTotal();

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);
        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        return Map(order);
    }

    public async Task<OrderResponse> AdvanceStatusAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(candidate => candidate.Items)
            .FirstOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        if (order is null)
        {
            throw new OrderNotFoundException(id);
        }

        order.AdvanceStatus();
        await _context.SaveChangesAsync(cancellationToken);
        return Map(order);
    }

    public async Task<OrderResponse> CancelAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(candidate => candidate.Items)
            .FirstOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        if (order is null)
        {
            throw new OrderNotFoundException(id);
        }

        order.Cancel();
        await _context.SaveChangesAsync(cancellationToken);
        return Map(order);
    }

    private static OrderResponse Map(Order order)
    {
        return new OrderResponse(
            order.Id,
            order.CustomerName,
            order.CustomerDocument,
            order.PostalCode,
            order.Status.ToString(),
            order.Total,
            order.CreatedAtUtc,
            order.UpdatedAtUtc,
            order.Items.Select(item => new OrderItemResponse(
                item.Id,
                item.ProductId,
                item.ProductNameSnapshot,
                item.Quantity,
                item.UnitPriceSnapshot,
                item.Subtotal,
                item.Observation)).ToArray());
    }
}

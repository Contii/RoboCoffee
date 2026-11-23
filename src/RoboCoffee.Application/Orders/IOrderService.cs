using RoboCoffee.Contracts.Orders;
using RoboCoffee.Contracts.Catalog;

namespace RoboCoffee.Application.Orders;

public interface IOrderService
{
    Task<PagedResult<OrderResponse>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<OrderResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken);

    Task<OrderResponse> AdvanceStatusAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<OrderResponse> CancelAsync(
        Guid id,
        CancellationToken cancellationToken);
}

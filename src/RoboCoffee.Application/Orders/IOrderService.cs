using RoboCoffee.Contracts.Orders;

namespace RoboCoffee.Application.Orders;

public interface IOrderService
{
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

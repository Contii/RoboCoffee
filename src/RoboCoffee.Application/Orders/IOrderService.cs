using RoboCoffee.Contracts.Orders;

namespace RoboCoffee.Application.Orders;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken);
}

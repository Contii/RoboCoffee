using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RoboCoffee.Application.Orders;
using RoboCoffee.Contracts.Orders;

namespace RoboCoffee.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;
    private readonly IValidator<CreateOrderRequest> _createValidator;

    public OrdersController(
        IOrderService orders,
        IValidator<CreateOrderRequest> createValidator)
    {
        _orders = orders;
        _createValidator = createValidator;
    }

    [HttpPost]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderResponse>> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);
        var order = await _orders.CreateAsync(request, cancellationToken);
        return Created($"/api/orders/{order.Id}", order);
    }

    [HttpPost("{id:guid}/advance")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderResponse>> AdvanceStatus(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await _orders.AdvanceStatusAsync(id, cancellationToken);
        return Ok(order);
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderResponse>> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await _orders.CancelAsync(id, cancellationToken);
        return Ok(order);
    }
}

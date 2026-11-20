using FluentValidation;
using RoboCoffee.Contracts.Orders;

namespace RoboCoffee.Application.Orders;

public sealed class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(request => request.CustomerName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.CustomerDocument)
            .MaximumLength(20)
            .When(request => request.CustomerDocument is not null);

        RuleFor(request => request.PostalCode)
            .MaximumLength(10)
            .When(request => request.PostalCode is not null);

        RuleFor(request => request.Items)
            .NotNull()
            .Must(items => items is { Count: > 0 })
            .WithMessage("Order must contain at least one item.");

        RuleForEach(request => request.Items)
            .ChildRules(item =>
            {
                item.RuleFor(orderItem => orderItem.ProductId)
                    .NotEqual(Guid.Empty);
                item.RuleFor(orderItem => orderItem.Quantity)
                    .GreaterThan(0);
                item.RuleFor(orderItem => orderItem.Observation)
                    .MaximumLength(500)
                    .When(orderItem => orderItem.Observation is not null);
            });
    }
}

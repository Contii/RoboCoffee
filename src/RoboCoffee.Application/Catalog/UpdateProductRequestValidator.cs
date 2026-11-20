using FluentValidation;
using RoboCoffee.Contracts.Catalog;

namespace RoboCoffee.Application.Catalog;

public sealed class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Category)
            .Must(category => Enum.TryParse<RoboCoffee.Domain.Catalog.ProductCategory>(category, true, out _))
            .WithMessage("Unknown product category.");

        RuleFor(request => request.Size)
            .Must(size => Enum.TryParse<RoboCoffee.Domain.Catalog.ProductSize>(size, true, out _))
            .WithMessage("Unknown product size.");

        RuleFor(request => request.Price)
            .GreaterThan(0);

        RuleFor(request => request.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .When(request => request.StockQuantity.HasValue);
    }
}

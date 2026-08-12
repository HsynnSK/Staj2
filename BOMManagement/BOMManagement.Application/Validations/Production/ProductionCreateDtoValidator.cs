using BOMManagement.Application.DTOs.Production;
using FluentValidation;

namespace BOMManagement.Application.Validations.Production;

public class ProductionCreateDtoValidator : AbstractValidator<ProductionCreateDto>
{
    public ProductionCreateDtoValidator()
    {
        RuleFor(x => x.ProductionNo)
            .NotEmpty().WithMessage("Production number cannot be empty.")
            .MaximumLength(50).WithMessage("Production number cannot exceed 50 characters.");

        RuleFor(x => x.ParentProductionNo)
            .MaximumLength(50).WithMessage("Parent production number cannot exceed 50 characters.");

        RuleFor(x => x.ItemCode)
            .NotEmpty().WithMessage("Item code is required.")
            .MaximumLength(50).WithMessage("Item code cannot exceed 50 characters.");

        RuleFor(x => x.TargetQuantity)
            .GreaterThan(0).WithMessage("Target quantity must be greater than 0.");
    }
}

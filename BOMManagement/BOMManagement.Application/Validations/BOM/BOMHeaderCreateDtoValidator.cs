using BOMManagement.Application.DTOs.BOM;
using FluentValidation;

namespace BOMManagement.Application.Validations.BOM;

public class BOMHeaderCreateDtoValidator : AbstractValidator<BOMHeaderCreateDto>
{
    public BOMHeaderCreateDtoValidator()
    {
        RuleFor(x => x.BOMCode)
            .NotEmpty().WithMessage("BOM code cannot be empty.")
            .MaximumLength(50).WithMessage("BOM code cannot exceed 50 characters.");

        RuleFor(x => x.ParentItemCode)
            .NotEmpty().WithMessage("Parent item code cannot be empty.")
            .MaximumLength(50).WithMessage("Parent item code cannot exceed 50 characters.");

        RuleFor(x => x.Version)
            .GreaterThan(0).WithMessage("Version must be greater than 0.");

        RuleFor(x => x.BaseQuantity)
            .GreaterThan(0).WithMessage("Base quantity must be greater than 0.");

        RuleFor(x => x.UnitCode)
            .IsInEnum().WithMessage("Invalid unit type.");

        RuleFor(x => x.Lines)
            .NotEmpty().WithMessage("At least one BOM line must be defined.");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.ChildItemCode)
                .NotEmpty().WithMessage("Child item code is required.");
            line.RuleFor(l => l.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            line.RuleFor(l => l.UnitCode)
                .IsInEnum().WithMessage("Invalid unit type.");
            line.RuleFor(l => l.ConsumptionWarehouse)
                .NotEmpty().WithMessage("Consumption warehouse is required.");
        });
    }
}

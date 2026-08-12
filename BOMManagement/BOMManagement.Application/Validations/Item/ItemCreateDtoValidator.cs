using BOMManagement.Application.DTOs.Item;
using FluentValidation;

namespace BOMManagement.Application.Validations.Item;

public class ItemCreateDtoValidator : AbstractValidator<ItemCreateDto>
{
    public ItemCreateDtoValidator()
    {
        RuleFor(x => x.ItemCode)
            .NotEmpty().WithMessage("Item code cannot be empty.")
            .MaximumLength(50).WithMessage("Item code cannot exceed 50 characters.");

        RuleFor(x => x.ItemName)
            .NotEmpty().WithMessage("Item name cannot be empty.")
            .MaximumLength(200).WithMessage("Item name cannot exceed 200 characters.");

        RuleFor(x => x.ItemType)
            .IsInEnum().WithMessage("Invalid item type.");

        RuleFor(x => x.MainUnit)
            .IsInEnum().WithMessage("Invalid unit type.");

        RuleFor(x => x.DefaultWarehouseId)
            .GreaterThan(0).When(x => x.DefaultWarehouseId.HasValue).WithMessage("Invalid warehouse.");

        RuleFor(x => x.MinimumStockLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum stock level must be greater than or equal to 0.");

        RuleFor(x => x.MaterialTypeId)
            .GreaterThan(0).WithMessage("Material type must be specified.");
    }
}

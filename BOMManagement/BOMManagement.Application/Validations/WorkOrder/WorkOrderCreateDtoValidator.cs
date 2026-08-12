using BOMManagement.Application.DTOs.WorkOrder;
using FluentValidation;

namespace BOMManagement.Application.Validations.WorkOrder;

public class WorkOrderCreateDtoValidator : AbstractValidator<WorkOrderCreateDto>
{
    public WorkOrderCreateDtoValidator()
    {
        RuleFor(x => x.ItemCode)
            .NotEmpty().WithMessage("Item code is required.")
            .MaximumLength(50).WithMessage("Item code cannot exceed 50 characters.");

        RuleFor(x => x.WorkOrderNo)
            .NotEmpty().WithMessage("Work order number is required.")
            .MaximumLength(50).WithMessage("Work order number cannot exceed 50 characters.");

        RuleFor(x => x.WarehouseCode)
            .NotEmpty().WithMessage("Warehouse code is required.")
            .MaximumLength(50).WithMessage("Warehouse code cannot exceed 50 characters.");

        RuleFor(x => x.ManufacturingCode)
            .NotEmpty().WithMessage("Manufacturing code is required.")
            .MaximumLength(50).WithMessage("Manufacturing code cannot exceed 50 characters.");

        RuleFor(x => x.WorkCode)
            .NotEmpty().WithMessage("Work code is required.")
            .MaximumLength(50).WithMessage("Work code cannot exceed 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.WorkCenterCode)
            .MaximumLength(50).WithMessage("Work center code cannot exceed 50 characters.");

        RuleFor(x => x.SetupTime)
            .GreaterThanOrEqualTo(0).WithMessage("Setup time cannot be negative.");

        RuleFor(x => x.RunTime)
            .GreaterThanOrEqualTo(0).WithMessage("Run time cannot be negative.");
    }
}

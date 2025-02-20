using CompanyManagement.InputModels;
using FluentValidation;

public class EmployeeValidator : AbstractValidator<EmployeeInputModel>
{
    public EmployeeValidator()
    {
        RuleFor(e => e.FullName)
            .NotEmpty().WithMessage("Full Name is required")
            .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters");

        RuleFor(e => e.Position)
            .NotEmpty().WithMessage("Position is required");

        RuleFor(e => e.CompanyIds)
            .NotNull().WithMessage("CompanyIds list cannot be null")
            .Must(ids => ids.Count > 0).WithMessage("At least one Company ID must be provided");

        RuleFor(e => e.ProjectIds)
            .NotNull().WithMessage("ProjectIds list cannot be null")
            .Must(ids => ids.Count > 0).WithMessage("At least one Project ID must be provided");
    }
}

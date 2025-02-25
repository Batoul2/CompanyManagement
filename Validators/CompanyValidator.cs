using CompanyManagement.InputModels;
using FluentValidation;

public class CompanyValidator : AbstractValidator<CompanyInputModel>
{
    public CompanyValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(150).WithMessage("Company name cannot exceed 150 characters.");
    }
}

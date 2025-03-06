using CompanyManagement.API.InputModels;
using FluentValidation;

namespace CompanyManagement.API.Validators
{
    public class ProjectValidator : AbstractValidator<ProjectInputModel>
    {
        public ProjectValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty().WithMessage("Project Title is required")
                .MaximumLength(200).WithMessage("Project Title cannot exceed 200 characters");

            RuleFor(p => p.Duration)
                .GreaterThan(TimeSpan.Zero).WithMessage("Duration must be greater than zero");
        }
    }
}


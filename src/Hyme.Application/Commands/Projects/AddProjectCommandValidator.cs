using FluentValidation;

namespace Hyme.Application.Commands.Projects
{
    public class AddProjectCommandValidator : AbstractValidator<AddProjectCommand>
    {
        public AddProjectCommandValidator()
        {
            RuleFor(s => s.Title).NotEmpty().MaximumLength(100);
            RuleFor(s => s.ShortDescription).NotEmpty().MaximumLength(1000);
            RuleFor(s => s.ProjectDescription).NotEmpty().MaximumLength(5000);
        }
    }
}

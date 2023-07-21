using FluentValidation;

namespace Hyme.Application.Commands.Users
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(u => u.Name).NotEmpty();
        }
    }
}

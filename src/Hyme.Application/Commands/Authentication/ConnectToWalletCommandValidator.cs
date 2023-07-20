using FluentValidation;

namespace Hyme.Application.Commands.Authentication
{
    public class ConnectToWalletCommandValidator : AbstractValidator<ConnectToWalletCommand>
    {

        public ConnectToWalletCommandValidator()
        {
            RuleFor(s => s.WalletAddress).Length(42).WithMessage("Invalid Wallet Address");
            RuleFor(s => s.Signature).Length(132).WithMessage("Invalid Signature");
            RuleFor(s => s.Message).NotEmpty();
        }
    }
}

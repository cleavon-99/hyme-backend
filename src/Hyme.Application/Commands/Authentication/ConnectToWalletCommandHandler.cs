using FluentResults;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Errors;
using Hyme.Application.Services;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;

namespace Hyme.Application.Commands.Authentication
{
    public class ConnectToWalletCommandHandler : IRequestHandler<ConnectToWalletCommand, Result<AuthenticationResponse>>
    {
        private readonly IWalletValidationService _walletValidationService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConnectToWalletCommandHandler(
            IWalletValidationService walletValidationService, 
            ITokenGenerator tokenGenerator,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _walletValidationService = walletValidationService;
            _tokenGenerator = tokenGenerator;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AuthenticationResponse>> Handle(ConnectToWalletCommand request, CancellationToken cancellationToken)
        {
            bool valid = _walletValidationService
                .Validate(request.Message, request.Signature, request.WalletAddress);

            if (!valid)
                return Result.Fail(new InvalidWalletError());

            User? userProfile =
                await _userRepository.GetByWalletAddress(new WalletAddress(request.WalletAddress));

            if(userProfile is null)
            {
                userProfile = User.Create(
                    new UserId(Guid.NewGuid()), 
                    new WalletAddress(request.WalletAddress)
                    );

                await _userRepository.AddAsync(userProfile);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            string token = _tokenGenerator.GenerateToken(userProfile);
            return new AuthenticationResponse(userProfile.Id.Value, token);
        }
    }
}

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
        private readonly IUserRepository _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConnectToWalletCommandHandler(
            IWalletValidationService walletValidationService, 
            ITokenGenerator tokenGenerator,
            IUserRepository userProfileRepository,
            IUnitOfWork unitOfWork)
        {
            _walletValidationService = walletValidationService;
            _tokenGenerator = tokenGenerator;
            _userProfileRepository = userProfileRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AuthenticationResponse>> Handle(ConnectToWalletCommand request, CancellationToken cancellationToken)
        {
            bool valid = _walletValidationService
                .Validate(request.Message, request.Signature, request.WalletAddress);

            if (!valid)
                return Result.Fail(new InvalidWalletError());

            User? userProfile =
                await _userProfileRepository.GetByWalletAddress(new WalletAddress(request.WalletAddress));

            if(userProfile is null)
            {
                userProfile = new(
                    new UserId(Guid.NewGuid()), 
                    new WalletAddress(request.WalletAddress), 
                    DateTime.UtcNow);
                await _userProfileRepository.AddAsync(userProfile);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            string token = _tokenGenerator.GenerateToken(userProfile);
            return new AuthenticationResponse(userProfile.Id.Value, token);
        }
    }
}

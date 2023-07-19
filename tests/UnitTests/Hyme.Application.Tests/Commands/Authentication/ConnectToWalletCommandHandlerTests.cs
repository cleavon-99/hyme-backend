using FluentAssertions;
using Hyme.Application.Commands.Authentication;
using Hyme.Application.Services;
using Hyme.Domain.Repositories;
using Moq;
using FluentResults.Extensions.FluentAssertions;
using Hyme.Shared.Errors;
using Hyme.Domain.ValueObjects;
using Hyme.Domain.Entities;
using Hyme.Application.DTOs.Response;

namespace Hyme.Application.Tests.Commands.Authentication
{
    public class ConnectToWalletCommandHandlerTests
    {

        private readonly Mock<IWalletValidationService> _walletValidationService;
        private readonly Mock<ITokenGenerator> _tokenGenerator;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;


        public ConnectToWalletCommandHandlerTests()
        {
            _walletValidationService = new();
            _tokenGenerator = new();
            _userRepository = new();
            _unitOfWork = new();
        }

        [Fact]
        public async Task Handle_ShouldInvokeValidationForWalletAddressAndSignature()
        {
            //Arrange
            ConnectToWalletCommand command = new("","","");
            ConnectToWalletCommandHandler sut = new(_walletValidationService.Object, _tokenGenerator.Object, _userRepository.Object, _unitOfWork.Object);
            
            //Act
            var result = await sut.Handle(command, CancellationToken.None);
            
            //Assert
            _walletValidationService.Verify(w => w.Validate(command.Message, command.Signature, command.WalletAddress));
        }

        [Fact]
        public async Task Handle_ShouldReturnInvalidWalletError_WhenValidationFails()
        {
            //Arrange
            ConnectToWalletCommand command = new("", "", "");
            _walletValidationService.Setup(s => s.Validate(command.Message, command.Signature, command.WalletAddress)).Returns(false);
            ConnectToWalletCommandHandler sut = new(_walletValidationService.Object, _tokenGenerator.Object, _userRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            result.Should().HaveReason(new InvalidWalletError());
        }

        [Fact]
        public async Task Handle_ShouldInvokeGetWalletByAddress_WhenWalletSignatureIsValid()
        {
            //Arrange
            ConnectToWalletCommand command = new("", "", "");
            _walletValidationService.Setup(s => s.Validate(command.Message, command.Signature, command.WalletAddress)).Returns(true);
            ConnectToWalletCommandHandler sut = new(_walletValidationService.Object, _tokenGenerator.Object, _userRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            _userRepository.Verify(u => u.GetByWalletAddress(new WalletAddress(command.WalletAddress)));
        }

        [Fact]
        public async Task Handle_ShouldAddAndSaveuser_WhenGetByWalletAddressReturnsNull()
        {
            //Arrange
            ConnectToWalletCommand command = new("", "", "");
            _walletValidationService.Setup(s => s.Validate(command.Message, command.Signature, command.WalletAddress)).Returns(true);
            ConnectToWalletCommandHandler sut = new(_walletValidationService.Object, _tokenGenerator.Object, _userRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            _userRepository.Verify(u => u.AddAsync(It.IsAny<User>()));
            _unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldGenerateToken_WhenWalletAddressIsValid()
        {
            //Arrange
            ConnectToWalletCommand command = new("", "", "");
            _walletValidationService.Setup(s => s.Validate(command.Message, command.Signature, command.WalletAddress)).Returns(true);
            ConnectToWalletCommandHandler sut = new(_walletValidationService.Object, _tokenGenerator.Object, _userRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            _tokenGenerator.Verify(u => u.GenerateToken(It.IsAny<User>()));
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessWithAuthResponseValueType_WhenWalletAddressIsValid()
        {
            //Arrange
            ConnectToWalletCommand command = new("", "", "");
            _walletValidationService.Setup(s => s.Validate(command.Message, command.Signature, command.WalletAddress)).Returns(true);
            ConnectToWalletCommandHandler sut = new(_walletValidationService.Object, _tokenGenerator.Object, _userRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeSuccess();
            result.Value.Should().BeOfType<AuthenticationResponse>();
        }
    }
}


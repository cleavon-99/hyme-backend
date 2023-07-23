using FluentAssertions;
using Hyme.Application.Commands.Authentication;
using Hyme.Application.Services;
using Hyme.Domain.Repositories;
using Moq;
using FluentResults.Extensions.FluentAssertions;
using Hyme.Domain.ValueObjects;
using Hyme.Domain.Entities;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Errors;
using TestUtilities.Commands;

namespace Hyme.Application.Tests.Commands.Authentication
{
    public class ConnectToWalletCommandHandlerTests
    {

        private readonly Mock<IWalletValidationService> _mockWalletValidationService;
        private readonly Mock<ITokenGenerator> _mockTokenGenerator;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly ConnectToWalletCommandHandler _sut;

        public ConnectToWalletCommandHandlerTests()
        {
            _mockWalletValidationService = new();
            _mockTokenGenerator = new();
            _mockUserRepository = new();
            _mockUnitOfWork = new();
            _sut = new(
                _mockWalletValidationService.Object,
                _mockTokenGenerator.Object,
                _mockUserRepository.Object,
                _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldInvokeValidationForWalletAddressAndSignature()
        {
            //Arrange
            ConnectToWalletCommand command = AuthenticationUtilities.ConnectToWalletCommand();

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);
            
            //Assert
            _mockWalletValidationService.Verify(w => w.Validate(
                command.Message,
                command.Signature,
                command.WalletAddress));
        }

        [Fact]
        public async Task Handle_ShouldReturnInvalidWalletError_WhenValidationFails()
        {
            //Arrange
            ConnectToWalletCommand command = AuthenticationUtilities.ConnectToWalletCommand();
            _mockWalletValidationService.Setup(s => s.Validate(
                command.Message,
                command.Signature,
                command.WalletAddress)).Returns(false);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            result.Should().HaveReason(new InvalidWalletError());
        }

        [Fact]
        public async Task Handle_ShouldInvokeGetWalletByAddress_WhenWalletSignatureIsValid()
        {
            //Arrange
            ConnectToWalletCommand command = AuthenticationUtilities.ConnectToWalletCommand();
            _mockWalletValidationService.Setup(s => s.Validate(
                command.Message,
                command.Signature,
                command.WalletAddress)).Returns(true);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockUserRepository.Verify(u => u.GetByWalletAddress(new WalletAddress(command.WalletAddress)));
        }

        [Fact]
        public async Task Handle_ShouldAddAndSaveuser_WhenGetByWalletAddressReturnsNull()
        {
            //Arrange
            ConnectToWalletCommand command = AuthenticationUtilities.ConnectToWalletCommand();
            _mockWalletValidationService.Setup(s => s.Validate(
                command.Message,
                command.Signature,
                command.WalletAddress)).Returns(true);
          
            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockUserRepository.Verify(u => u.AddAsync(It.IsAny<User>()));
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldGenerateToken_WhenWalletAddressIsValid()
        {
            //Arrange
            ConnectToWalletCommand command = AuthenticationUtilities.ConnectToWalletCommand();
            _mockWalletValidationService.Setup(s => s.Validate(
                command.Message,
                command.Signature,
                command.WalletAddress)).Returns(true);
           
            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockTokenGenerator.Verify(u => u.GenerateToken(It.IsAny<User>()));
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessWithAuthResponseValueType_WhenWalletAddressIsValid()
        {
            //Arrange
            ConnectToWalletCommand command = AuthenticationUtilities.ConnectToWalletCommand();
            _mockWalletValidationService.Setup(s => s.Validate(
                command.Message,
                command.Signature,
                command.WalletAddress)).Returns(true);

           
            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeSuccess();
            result.Value.Should().BeOfType<AuthenticationResponse>();
        }
    }
}


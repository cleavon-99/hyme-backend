using FluentAssertions;
using Hyme.Application.Commands.UserProfiles;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using FluentResults.Extensions.FluentAssertions;
using Hyme.Domain.Entities;
using Hyme.Application.Errors;

namespace Hyme.Application.Tests.Commands.UserProfiles
{
    public class UpdateUserProfileCommandHandlerTests
    {

        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;

        public UpdateUserProfileCommandHandlerTests()
        {
            _userRepository = new();
            _unitOfWork = new();
        }


        [Fact]
        public async Task Handle_ShouldInvokeRepository_GetByIdAsync()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            UserId userId = new(id);
            UpdateUserProfileCommand command = new(id, "Arjay");
            UpdateUserProfileCommandHandler sut = new(_userRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            _userRepository.Verify(u => u.GetByIdAsync(userId));
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureWithUserProfileNotFoundError_WhenRepositoryReturnsNullValue()
        {
            //Arrange
            Guid id = Guid.NewGuid();

            UpdateUserProfileCommand command = new(id, "Arjay");
            UpdateUserProfileCommandHandler sut = new(_userRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            result.Should().HaveReason(new UserNotFoundError(id));
        }

        [Fact]
        public async Task Handle_ShouldInvokeUserProfileUpdate_WhenRepostoryReturnsNonNullValue()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            UserId userId = new(id);
            User user = new(userId, new WalletAddress("0x000"), DateTime.UtcNow);
            _userRepository.Setup(s => s.GetByIdAsync(userId)).ReturnsAsync(user);
            UpdateUserProfileCommand command = new(id, "Arjay");
            UpdateUserProfileCommandHandler sut = new(_userRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            user.Name.Should().Be("Arjay");
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveAsync_AndReturnsSuccessResult()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            UserId userId = new(id);
            User user = new(userId, new WalletAddress("0x000"), DateTime.UtcNow);
            _userRepository.Setup(s => s.GetByIdAsync(userId)).ReturnsAsync(user);
            UpdateUserProfileCommand command = new(id, "Arjay");
            UpdateUserProfileCommandHandler sut = new(_userRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
            result.Should().BeSuccess();
        }     

    }
}

using FluentAssertions;
using Hyme.Application.Commands.Projects;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using TestUtilities.Constants;
using FluentResults.Extensions.FluentAssertions;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using TestUtilities.Repository;
using Hyme.Domain.Primitives;

namespace Hyme.Application.Tests.Commands.Projects
{
    public class PublishProjectCommandHandlerTests
    {

        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly PublishProjectCommandHandler _sut;

        public PublishProjectCommandHandlerTests()
        {
            _mockProjectRepository = new();
            _mockUnitOfWork = new();
            _sut = new(_mockProjectRepository.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldInvoke_RepositoryGetProjectByOwnerIdAsync()
        {
            //Arrange
            UserId userId = Constants.User.UserId;
            PublishProjectCommand command = new(userId.Value);


            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockProjectRepository.Verify(p => p.GetByOwnerIdAsync(userId));
        }

        [Fact]
        public async Task Handle_ShouldReturnProjectNotFoundResult_WhenRepositoryReturnsNull()
        {
            //Arrange
            UserId userId = Constants.User.UserId;
            PublishProjectCommand command = new(userId.Value);


            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            result.Should().HaveReason(new ProjectNotFoundError(command.UserId));
        }

        [Fact]
        public async Task Handle_ShouldPublishTheProject_WhenRepositoryReturnsAValue()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            PublishProjectCommand command = new(project.OwnerId.Value);
            _mockProjectRepository.Setup(p => p.GetByOwnerIdAsync(project.OwnerId)).ReturnsAsync(project);


            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            project.Status.Should().Be(PublishStatus.InReview);
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_WhenRepositoryReturnsAValue()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            PublishProjectCommand command = new(project.OwnerId.Value);
            _mockProjectRepository.Setup(p => p.GetByOwnerIdAsync(project.OwnerId)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenRepositoryReturnsAValue()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            PublishProjectCommand command = new(project.OwnerId.Value);
            _mockProjectRepository.Setup(p => p.GetByOwnerIdAsync(project.OwnerId)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeSuccess();
        }
    }
}

using Hyme.Application.Commands.Projects;
using Hyme.Application.Services;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using FluentResults.Extensions.FluentAssertions;
using Moq;
using TestUtilities.Repository;
using Hyme.Application.Errors;
using FluentAssertions;

namespace Hyme.Application.Tests.Commands.Projects
{
    public class DeleteTrailerCommandHandlerTests
    {

        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly DeleteTrailerCommandHandler _sut;

        public DeleteTrailerCommandHandlerTests()
        {
            _mockProjectRepository = new();
            _mockUnitOfWork = new();
            _mockBlobService = new();
            _sut = new(_mockProjectRepository.Object, _mockUnitOfWork.Object, _mockBlobService.Object);
        }

        [Fact]
        public async Task Handle_ShouldInvoke_RepositoryGetByIdAsync()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();

            DeleteTrailerCommand command = new(project.Id.Value);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockProjectRepository.Verify(p => p.GetByIdAsync(project.Id));
        }

        [Fact]
        public async Task Handle_ShouldReturnProjectNotFoundErrorResult_WhenRepositoryReturnsNull()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();

            DeleteTrailerCommand command = new(project.Id.Value);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            result.Should().HaveReason(new ProjectNotFoundError(project.Id.Value));
        }

        [Fact]
        public async Task Handle_ShouldInvokeBlobServiceDeleteTrailer_WhenTrailerIsNotEmpty()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateTrailer("New trailer");
            DeleteTrailerCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockBlobService.Verify(b => b.DeleteVideoAsync("New trailer"));
        }

        [Fact]
        public async Task Handle_ShouldDeleteTheTrailer_WhenRepositoryReturnsAValue()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateTrailer("New trailer.mp4");

            DeleteTrailerCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            project.Trailer.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_WhenRepositoryReturnsAValue()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateTrailer("New trailer.mp4");

            DeleteTrailerCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

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
            project.UpdateTrailer("New trailer.mp4");

            DeleteTrailerCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeSuccess();
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenTrailerIsAlreadyEmpty()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
   
            DeleteTrailerCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Never());
        }


    }
}

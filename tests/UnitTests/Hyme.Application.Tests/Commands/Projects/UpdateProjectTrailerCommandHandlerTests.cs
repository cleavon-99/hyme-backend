using Hyme.Application.Commands.Projects;
using Hyme.Application.Services;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using FluentResults.Extensions.FluentAssertions;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using TestUtilities.Repository;
using FluentAssertions;

namespace Hyme.Application.Tests.Commands.Projects
{
    public class UpdateProjectTrailerCommandHandlerTests 
    {

        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly UpdateProjectTrailerCommandHandler _sut;

        public UpdateProjectTrailerCommandHandlerTests()
        {
            _mockProjectRepository = new();
            _mockUnitOfWork = new();
            _mockBlobService = new();
            _sut = new(_mockProjectRepository.Object, _mockUnitOfWork.Object, _mockBlobService.Object);
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryGetByIdAsync()
        {
            //Arrange
            UpdateProjectTrailerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "newTrailerName.mp4");

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockProjectRepository.Verify(v => v.GetByIdAsync(new ProjectId(command.ProjectId)));
        }

        [Fact]
        public async Task Handle_ShouldReturnProjectNotFoundError_WhenRepositoryReturnsNull()
        {
            //Arrange
            UpdateProjectTrailerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "newTrailerName.mp4");

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            result.Should().HaveReason(new ProjectNotFoundError(command.ProjectId));
        }

        [Fact]
        public async Task Handle_ShouldDeleteOldProjectTrailer_ProjectHasAlreadyATrailer()
        {
            //Arrange
            UpdateProjectTrailerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "newTrailerName.mp4");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateTrailer("oldTrailer.mp4");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId)))
                .ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockBlobService.Verify(b => b.DeleteVideoAsync("oldTrailer.mp4"));
        }

        [Fact]
        public async Task Handle_ShouldUpdateTheProjectTrailer_WhenRepositoryReturnsAValue()
        {
            //Arrange
            UpdateProjectTrailerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "newTrailerName.mp4");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateTrailer("oldTrailer.mp4");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId)))
                .ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            project.Trailer.Should().Be(command.FileName);
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_WhenRepositoryReturnsAValue()
        {
            //Arrange
            UpdateProjectTrailerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "newTrailerName.mp4");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateTrailer("oldTrailer.mp4");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId)))
                .ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldUploadTheVideoInBlobStorage_WhenRepositoryReturnsAValue()
        {
            //Arrange
            UpdateProjectTrailerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "newTrailerName.mp4");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateTrailer("oldTrailer.mp4");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId)))
                .ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockBlobService.Verify(b => b.UploadVideoAsync(command.Trailer, command.FileName));
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenRepositoryReturnsAValue()
        {
            //Arrange
            UpdateProjectTrailerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "newTrailerName.mp4");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateTrailer("oldTrailer.mp4");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId)))
                .ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeSuccess();
        }


    }
}

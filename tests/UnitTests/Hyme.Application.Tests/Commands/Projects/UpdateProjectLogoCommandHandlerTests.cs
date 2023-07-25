using Hyme.Application.Commands.Projects;
using Hyme.Application.Services;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using FluentResults.Extensions.FluentAssertions;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using TestUtilities.Repository;
using System.ComponentModel.Design;
using FluentAssertions;

namespace Hyme.Application.Tests.Commands.Projects
{
    public class UpdateProjectLogoCommandHandlerTests
    {

        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UpdateProjectLogoCommandHandler _sut;

        public UpdateProjectLogoCommandHandlerTests()
        {
            _mockProjectRepository = new();
            _mockBlobService = new();
            _mockUnitOfWork = new();
            _sut = new(_mockProjectRepository.Object, _mockBlobService.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldQueryRepository_GetProjectByIdAsync()
        {
            //Arrange
            UpdateProjectLogoCommand command = new UpdateProjectLogoCommand(Guid.NewGuid(), Array.Empty<byte>(), "logo.png");

            //Act
            await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockProjectRepository.Verify(m => m.GetByIdAsync(new ProjectId(command.ProjectId)));
        }

        [Fact]
        public async Task Handle_ShouldReturnFailProjectNotFoundError_WhenRepositoryReturnsNull()
        {
            //Arrange
            UpdateProjectLogoCommand command = new UpdateProjectLogoCommand(Guid.NewGuid(), Array.Empty<byte>(), "logo.png");

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            result.Should().HaveReason(new ProjectNotFoundError(command.ProjectId));
        }

        [Fact]
        public async Task Handle_ShouldDeletePreviousLogo_WhenProjectHasALogo()
        {
            //Arrange
            UpdateProjectLogoCommand command = new UpdateProjectLogoCommand(Guid.NewGuid(), Array.Empty<byte>(), "logo.png");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateLogo("currentLogo");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId))).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockBlobService.Verify(b => b.DeleteImageAsync("currentLogo"));
        }

        [Fact]
        public async Task Handle_ShouldUpdateTheLogo_IfProjectIsNotNull()
        {
            //Arrange
            UpdateProjectLogoCommand command = new UpdateProjectLogoCommand(Guid.NewGuid(), Array.Empty<byte>(), "logo.png");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateLogo("currentLogo");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId))).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            project.Logo.Should().Be(command.FileName);
        }

        [Fact]
        public async Task Handle_ShouldInvokeBlobServiceUploadImageAsync_WhenProjectIsNotNull()
        {
            //Arrange
            UpdateProjectLogoCommand command = new UpdateProjectLogoCommand(Guid.NewGuid(), Array.Empty<byte>(), "logo.png");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateLogo("currentLogo");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId))).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockBlobService.Verify(m => m.UploadImageAsync(command.Logo, command.FileName));
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_WhenProjectIsNotNull()
        {
            //Arrange
            UpdateProjectLogoCommand command = new UpdateProjectLogoCommand(Guid.NewGuid(), Array.Empty<byte>(), "logo.png");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateLogo("currentLogo");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId))).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessWhenProjectIsNotNull()
        {
            //Arrange
            UpdateProjectLogoCommand command = new UpdateProjectLogoCommand(Guid.NewGuid(), Array.Empty<byte>(), "logo.png");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateLogo("currentLogo");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId))).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeSuccess();
        }




    }
}

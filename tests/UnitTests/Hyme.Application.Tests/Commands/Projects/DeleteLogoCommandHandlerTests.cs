using FluentAssertions;
using Hyme.Application.Commands.Projects;
using Hyme.Application.Services;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Moq;
using FluentResults.Extensions.FluentAssertions;
using TestUtilities.Repository;
using Hyme.Application.Errors;

namespace Hyme.Application.Tests.Commands.Projects
{
    public class DeleteLogoCommandHandlerTests
    {

        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly DeleteLogoCommandHandler _sut;

        public DeleteLogoCommandHandlerTests()
        {
            _mockProjectRepository = new();
            _mockUnitOfWork = new();
            _mockBlobService = new();
            _sut = new(_mockProjectRepository.Object, _mockUnitOfWork.Object, _mockBlobService.Object);
        }

        [Fact]
        public async Task Handle_ShouldInvokeRepositoryGetProjectByIdAsync()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            DeleteLogoCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(() => null);

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
            DeleteLogoCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(() => null);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            result.Should().HaveReason(new ProjectNotFoundError(command.ProjectId));
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenLogoIsAlreadyEmpty()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            DeleteLogoCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldInvokeBlobServiceDeleteImageAsync_WhenLogoIsPresent()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateLogo("Logo.png");
            DeleteLogoCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockBlobService.Verify(b => b.DeleteImageAsync("Logo.png"));
        }

        [Fact]
        public async Task Handle_ShouldDeleteTheLogoEntry_WhenLogoIsPresent()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateLogo("Logo.png");
            DeleteLogoCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            project.Logo.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_WhenLogoIsPresent()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateLogo("Logo.png");
            DeleteLogoCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
        }

        [Fact]
        public async Task Handl_ShouldReturnSuccessResult_WhenLogoIsPresent()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateLogo("Logo.png");
            DeleteLogoCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeSuccess();
        }
    }
}

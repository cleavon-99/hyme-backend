using AutoMapper;
using FluentAssertions;
using Hyme.Application.Commands.Projects;
using Hyme.Application.Services;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Moq;
using TestUtilities.Repository;
using FluentResults.Extensions.FluentAssertions;
using Hyme.Application.Errors;

namespace Hyme.Application.Tests.Commands.Projects
{
    public class DeleteBannerCommandHandlerTests
    {
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly DeleteBannerCommandHandler _sut;

        public DeleteBannerCommandHandlerTests()
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
            DeleteBannerCommand command = new(project.Id.Value);
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
            DeleteBannerCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(() => null);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            result.Should().HaveReason(new ProjectNotFoundError(command.ProjectId));
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenProjectBannerIsAlreadyEmpty()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            DeleteBannerCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldInvokeBlobServiceDeleteImageAsync_WhenProjectBannerIsNotEmpty()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateBanner("Current banner.png");
            DeleteBannerCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockBlobService.Verify(b => b.DeleteImageAsync("Current banner.png"));
        }

        [Fact]
        public async Task Handle_ShouldActullyDeleteBanner_WhenProjectBannerIsNotEmpty()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateBanner("Current banner.png");
            DeleteBannerCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            project.Banner.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_WhenProjectBannerIsNotEmpty()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateBanner("Current banner.png");
            DeleteBannerCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenProjectBannerIsNotEmpty()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateBanner("Current banner.png");
            DeleteBannerCommand command = new(project.Id.Value);
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeSuccess();
        }
    }
}

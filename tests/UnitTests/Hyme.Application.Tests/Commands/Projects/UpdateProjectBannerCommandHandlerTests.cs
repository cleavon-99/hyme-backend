using FluentAssertions;
using Hyme.Application.Commands.Projects;
using Hyme.Application.Services;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using FluentResults.Extensions.FluentAssertions;
using Hyme.Application.Errors;
using TestUtilities.Repository;
using Hyme.Domain.Entities;

namespace Hyme.Application.Tests.Commands.Projects
{
    public class UpdateProjectBannerCommandHandlerTests
    {


        private readonly Mock<IBlobService> _mockBlobService;
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UpdateProjectBannerCommandHandler _sut;

        public UpdateProjectBannerCommandHandlerTests()
        {
            _mockBlobService = new();
            _mockProjectRepository = new();
            _mockUnitOfWork = new();
            _sut = new(_mockProjectRepository.Object, _mockUnitOfWork.Object, _mockBlobService.Object);
        }

        [Fact]
        public async Task Handle_ShouldInvokeProjectRepositoryGetByIdAsync()
        {
            //Arrange
            UpdateProjectBannerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "banner.png");

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockProjectRepository.Verify(p => p.GetByIdAsync(new ProjectId(command.ProjectId)));
        }

        [Fact]
        public async Task Handle_ShouldReturnProjectNotFoundError_WhenRepositoryReturnsNull()
        {
            //Arrange
            UpdateProjectBannerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "banner.png");

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            result.Should().HaveReason(new ProjectNotFoundError(command.ProjectId));
        }

        [Fact]
        public async Task Handle_ShouldInvokeBlobServiceDeleteImageAsync_WhenProjectHasExistingBanner()
        {
            //Arrange
            UpdateProjectBannerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "banner.png");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateBanner("Oldbanner.png");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId)))
                .ReturnsAsync(project);
            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockBlobService.Verify(b => b.DeleteImageAsync("Oldbanner.png"));
        }

        [Fact]
        public async Task Handle_ShouldUpdateTheBanner_WhenRepositoryReturnsAValue()
        {
            //Arrange
            UpdateProjectBannerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "banner.png");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateBanner("Oldbanner.png");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId)))
                .ReturnsAsync(project);
            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            project.Banner.Should().Be(command.FileName);
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_WhenRepositoryReturnsAValue()
        {
            //Arrange
            UpdateProjectBannerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "banner.png");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateBanner("Oldbanner.png");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId)))
                .ReturnsAsync(project);
            
            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldUploadTheNewBannerToBlobStorage_IfWhenRepositoryReturnsAValue()
        {
            //Arrange
            UpdateProjectBannerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "banner.png");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateBanner("Oldbanner.png");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId)))
                .ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockBlobService.Verify(b => b.UploadImageAsync(command.Banner, command.FileName));
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenRepositoryReturnsAValue()
        {
            //Arrange
            UpdateProjectBannerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "banner.png");
            Project project = ProjectRepositoryUtilities.CreateProject();
            project.UpdateBanner("Oldbanner.png");
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId)))
                .ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeSuccess();
        }
    }
}

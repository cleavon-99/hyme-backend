using FluentAssertions;
using Hyme.Application.Commands.Projects;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using FluentResults.Extensions.FluentAssertions;
using Moq;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using TestUtilities.Commands;

namespace Hyme.Application.Tests.Commands.Projects
{
    public class RejectProjectCommandHandlerTests
    {
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly RejectProjectCommandHandler _sut;

        public RejectProjectCommandHandlerTests()
        {
            _mockProjectRepository = new();
            _mockUnitOfWork = new();
            _sut = new(_mockProjectRepository.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldInvokeGetProjectByIdAsync()
        {
            //Arrange
            RejectProjectCommand command = ProjectCommandsUtilities.RejectProjectCommand();
           
            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockProjectRepository.Verify(p => p.GetByIdAsync(new ProjectId(command.ProjectId)));
        }

        [Fact]
        public async Task Handle_ShouldReturnProjectNotFoundResult_WhenRepositoryReturnsNull()
        {
            //Arrange
            RejectProjectCommand command = ProjectCommandsUtilities.RejectProjectCommand();
           
            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().HaveReason(new ProjectNotFoundError(command.ProjectId));
        }

        [Fact]
        public async Task Handle_ShouldInvokeRejectProject_WhenRepositoryReturnsAValue()
        {
            //Arrange          
            RejectProjectCommand command = ProjectCommandsUtilities.RejectProjectCommand();
            Project project = ProjectCommandsUtilities.CreateProject();
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);


            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            project.Status.Should().Be(PublishStatus.Rejected);
            project.DateRejected.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_AndReturnsSuccessResult()
        {
            //Arrange
            RejectProjectCommand command = ProjectCommandsUtilities.RejectProjectCommand();
            Project project = ProjectCommandsUtilities.CreateProject();
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);
           
            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            project.Status.Should().Be(PublishStatus.Rejected);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
            result.Should().BeSuccess();
        }

    }
}

using FluentAssertions;
using Hyme.Application.Commands.Projects;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using FluentResults.Extensions.FluentAssertions;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using TestUtilities.Commands;
using TestUtilities.Repository;

namespace Hyme.Application.Tests.Commands.Projects
{
    public class ApproveProjectCommandHandlerTests
    {

        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly ApproveProjectCommandHandler _sut;

        public ApproveProjectCommandHandlerTests()
        {
            _mockProjectRepository = new();
            _mockUnitOfWork = new();
            _sut = new(_mockProjectRepository.Object, _mockUnitOfWork.Object);
        }


        [Fact]
        public async Task Handle_ShouldInvoke_ProjectRepositoryGetByIdAsync()
        {
            //Arrange
            ApproveProjectCommand command = ProjectCommandsUtilities.ApproveProjectCommand();
           
            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockProjectRepository.Verify(u => u.GetByIdAsync(new ProjectId(command.ProjectId)));
        }

        [Fact]
        public async Task Handle_ShouldReturnProjectNotFoundError_WhenRepositoryReturnsNull()
        {
            //Arrange
            ApproveProjectCommand command = ProjectCommandsUtilities.ApproveProjectCommand();
            
            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().HaveReason(new ProjectNotFoundError(command.ProjectId));
        }

        [Fact]
        public async Task Handle_ShouldApproveTheProject_WhenRepositoryReturnsAValue()
        {
            //Arrange
            ApproveProjectCommand command = ProjectCommandsUtilities.ApproveProjectCommand();
            Project project = ProjectRepositoryUtilities.CreateProject();
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId))).ReturnsAsync(project);
            
            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            project.Status.Should().Be(ProjectStatus.Approved);
            project.DateApproved.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_WhenProjectHadApproved()
        {
            //Arrange
            ApproveProjectCommand command = ProjectCommandsUtilities.ApproveProjectCommand();
            Project project = ProjectRepositoryUtilities.CreateProject();
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId))).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            project.Status.Should().Be(ProjectStatus.Approved);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
            result.Should().BeSuccess();
        }
    }
}

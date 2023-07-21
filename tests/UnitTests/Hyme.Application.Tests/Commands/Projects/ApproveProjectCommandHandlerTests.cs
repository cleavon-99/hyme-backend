using FluentAssertions;
using Hyme.Application.Commands.Projects;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using FluentResults.Extensions.FluentAssertions;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;

namespace Hyme.Application.Tests.Commands.Projects
{
    public class ApproveProjectCommandHandlerTests
    {

        private readonly Mock<IProjectRepository> _projectRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;

        public ApproveProjectCommandHandlerTests()
        {
            _projectRepository = new();
            _unitOfWork = new();
        }


        [Fact]
        public async Task Handle_ShouldInvoke_ProjectRepositoryGetByIdAsync()
        {
            //Arrange
            ApproveProjectCommand command = new(Guid.NewGuid());
            ApproveProjectCommandHandler sut = new(_projectRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            _projectRepository.Verify(u => u.GetByIdAsync(new ProjectId(command.ProjectId)));
        }

        [Fact]
        public async Task Handle_ShouldReturnProjectNotFoundError_WhenRepositoryReturnsNull()
        {
            //Arrange
            ApproveProjectCommand command = new(Guid.NewGuid());
            ApproveProjectCommandHandler sut = new(_projectRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().HaveReason(new ProjectNotFoundError(command.ProjectId));
        }

        [Fact]
        public async Task Handle_ShouldApproveTheProject_WhenRepositoryReturnsAValue()
        {
            //Arrange
            ApproveProjectCommand command = new(Guid.NewGuid());
            Project project = Project.Create(new ProjectId(command.ProjectId), new UserId(Guid.NewGuid()), "Title", "Logo", "Banner", "shortDescription", "ProjectDescription");
            _projectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId))).ReturnsAsync(project);
            
            ApproveProjectCommandHandler sut = new(_projectRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            project.Status.Should().Be(PublishStatus.Approved);
            project.DateApproved.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_WhenProjectHadApproved()
        {
            //Arrange
            ApproveProjectCommand command = new(Guid.NewGuid());
            Project project = Project.Create(new ProjectId(command.ProjectId), new UserId(Guid.NewGuid()), "Title", "Logo", "Banner", "shortDescription", "ProjectDescription");
            _projectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId))).ReturnsAsync(project);

            ApproveProjectCommandHandler sut = new(_projectRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            project.Status.Should().Be(PublishStatus.Approved);
            _unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
            result.Should().BeSuccess();
        }
    }
}

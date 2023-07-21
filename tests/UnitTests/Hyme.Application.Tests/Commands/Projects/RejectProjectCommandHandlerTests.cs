using FluentAssertions;
using Hyme.Application.Commands.Projects;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using FluentResults.Extensions.FluentAssertions;
using Moq;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;

namespace Hyme.Application.Tests.Commands.Projects
{
    public class RejectProjectCommandHandlerTests
    {
        private readonly Mock<IProjectRepository> _projectRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;

        public RejectProjectCommandHandlerTests()
        {
            _projectRepository = new();
            _unitOfWork = new();
        }

        [Fact]
        public async Task Handle_ShouldInvokeGetProjectByIdAsync()
        {
            //Arrange
            RejectProjectCommand command = new(Guid.NewGuid());
            RejectProjectCommandHandler sut = new(_projectRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            _projectRepository.Verify(p => p.GetByIdAsync(new ProjectId(command.ProjectId)));
        }

        [Fact]
        public async Task Handle_ShouldReturnProjectNotFoundResult_WhenRepositoryReturnsNull()
        {
            //Arrange
            RejectProjectCommand command = new(Guid.NewGuid());
            RejectProjectCommandHandler sut = new(_projectRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().HaveReason(new ProjectNotFoundError(command.ProjectId));
        }

        [Fact]
        public async Task Handle_ShouldInvokeRejectProject_WhenRepositoryReturnsAValue()
        {
            //Arrange
            
            RejectProjectCommand command = new(Guid.NewGuid());
            Project project = Project.Create(
                new ProjectId(command.ProjectId), 
                new UserId(Guid.NewGuid()), 
                "Title", 
                "Logo", 
                "Banner", 
                "Short Description", 
                "Project Description");
            _projectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);
            RejectProjectCommandHandler sut = new(_projectRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            project.Status.Should().Be(PublishStatus.Rejected);
            project.DateRejected.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_AndReturnsSuccessResult()
        {
            //Arrange
            RejectProjectCommand command = new(Guid.NewGuid());
            Project project = Project.Create(
                new ProjectId(command.ProjectId),
                new UserId(Guid.NewGuid()),
                "Title",
                "Logo",
                "Banner",
                "Short Description",
                "Project Description");
            _projectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);
            RejectProjectCommandHandler sut = new(_projectRepository.Object, _unitOfWork.Object);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            project.Status.Should().Be(PublishStatus.Rejected);
            _unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
            result.Should().BeSuccess();
        }

    }
}

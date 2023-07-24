using FluentAssertions;
using Hyme.Application.Commands.Projects;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using FluentResults.Extensions.FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using Hyme.Application.Errors;
using AutoMapper.Internal.Mappers;
using TestUtilities.Repository;
using Hyme.Domain.Entities;

namespace Hyme.Application.Tests.Commands.Projects
{
    public class UpdateProjectInfoCommandHandlerTests
    {

        private readonly UpdateProjectInfoCommandHandler _sut;
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public UpdateProjectInfoCommandHandlerTests()
        {
            _mockProjectRepository = new();
            _mockUnitOfWork = new();

            _sut = new(_mockProjectRepository.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldRetrieveProjectByIdQuery()
        {
            //Arrange
            UpdateProjectInfoCommand command = new(Guid.NewGuid(), "Title", "Short Description", "Project Description");

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockProjectRepository.Verify(p => p.GetByIdAsync(new ProjectId(command.ProjectId)));
        }

        [Fact]
        public async Task Handle_ShouldReturnFailResultWithProjectNotFoundReason_WhenRepositoryReturnsNull()
        {
            //Arrange
            UpdateProjectInfoCommand command = new(Guid.NewGuid(), "Title", "Short Description", "Project Description");

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            result.Should().HaveReason(new ProjectNotFoundError(command.ProjectId));
        }

        [Fact]
        public async Task Handle_ShouldUpdateTheProject_WhenRepositoryReturnsAValue()
        {
            //Arrange
            UpdateProjectInfoCommand command = new(Guid.NewGuid(), "Title Arjay", "Short Description Arjay", "Project Description Arjay");
            Project project = ProjectRepositoryUtilities.CreateProject();
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId))).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            project.Title.Should().Be(command.Title);
            project.ShortDescription.Should().Be(command.ShortDescription);
            project.ProjectDescription.Should().Be(command.ProjectDescription);
        }

        [Fact]
        public async Task Handle_ShouldInvokeSaveChangesAsync()
        {
            //Arrange
            UpdateProjectInfoCommand command = new(Guid.NewGuid(), "Title Arjay", "Short Description Arjay", "Project Description Arjay");
            Project project = ProjectRepositoryUtilities.CreateProject();
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId))).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_AfterSaveChanges()
        {
            //Arrange
            UpdateProjectInfoCommand command = new(Guid.NewGuid(), "Title Arjay", "Short Description Arjay", "Project Description Arjay");
            Project project = ProjectRepositoryUtilities.CreateProject();
            _mockProjectRepository.Setup(p => p.GetByIdAsync(new ProjectId(command.ProjectId))).ReturnsAsync(project);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
            result.Should().BeSuccess();
        }
    }
}

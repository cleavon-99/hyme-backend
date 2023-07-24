using AutoMapper;
using FluentAssertions;
using Hyme.Application.DTOs.Response;
using Hyme.Application.MappingProfiles;
using Hyme.Application.Queries.Projects;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using TestUtilities.Constants;
using TestUtilities.Repository;

namespace Hyme.Application.Tests.Queries.Projects
{
    public class GetProjectByIdQueryHandlerTests
    {

        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly IMapper _mapper;
        private readonly GetProjectByIdQueryHandler _sut;

        public GetProjectByIdQueryHandlerTests()
        { 
            _mockProjectRepository = new();
            MapperConfiguration mapperConfiguration = new(options => 
            {
                options.AddProfile<ProjectMappingProfiles>();
            });

            _mapper = mapperConfiguration.CreateMapper();
            _sut = new(_mockProjectRepository.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldInvokeRepository_GetByIdAsync()
        {
            //Arrange
            ProjectId projectId = Constants.Project.ProjectId;
            GetProjectByIdQuery query = new(projectId.Value);
            
            //Act
            ProjectResponse? project = await _sut.Handle(query, CancellationToken.None);

            //Assert
            _mockProjectRepository.Verify(p => p.GetByIdAsync(projectId));
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            //Arrange
            ProjectId projectId = Constants.Project.ProjectId;
            GetProjectByIdQuery query = new(projectId.Value);
            //Act
            ProjectResponse? project = await _sut.Handle(query, CancellationToken.None);

            //Assert
            project.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturnProjectResponse_WhenRepositoryReturnsAValue()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            GetProjectByIdQuery query = new(project.Id.Value);
            _mockProjectRepository.Setup(s => s.GetByIdAsync(project.Id)).ReturnsAsync(project);
            var sut = new GetProjectByIdQueryHandler(_mockProjectRepository.Object, _mapper);
            //Act
            ProjectResponse? projectResponse = await sut.Handle(query, CancellationToken.None);

            //Assert
            projectResponse.Should().NotBeNull();
            projectResponse!.Id.Should().Be(project.Id.Value);
        }
    }
}

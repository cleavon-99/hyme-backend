using AutoMapper;
using FluentAssertions;
using Hyme.Application.DTOs.Response;
using Hyme.Application.MappingProfiles;
using Hyme.Application.Queries.Projects;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using TestUtilities.Repository;

namespace Hyme.Application.Tests.Queries.Projects
{
    public class GetProjectByOwnerIdQueryHandlerTests
    {

        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly IMapper _mapper;
        private readonly GetProjectByOwnerIdQueryHandler _sut;

        public GetProjectByOwnerIdQueryHandlerTests()
        {
            MapperConfiguration mapperConfiguration = new(options =>
            {
                options.AddProfile<ProjectMappingProfiles>();
            });

            _mapper = mapperConfiguration.CreateMapper();
            _mockProjectRepository = new();
            _sut = new(_mockProjectRepository.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldInvoke_ProjectRepository_GetProjectByOwnerId()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();

            GetProjectByOwnerIdQuery query = new(project.OwnerId.Value);

            //Act
            ProjectResponse? response = await _sut.Handle(query, CancellationToken.None);

            //Assert
            _mockProjectRepository.Verify(p => p.GetByOwnerIdAsync(project.OwnerId));
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenProjectRepository_ReturnsNull()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();

            GetProjectByOwnerIdQuery query = new(project.OwnerId.Value);

            //Act
            ProjectResponse? response = await _sut.Handle(query, CancellationToken.None);

            //Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturnProjectResponse_WhenProjectRepository_ReturnsAValue()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();

            GetProjectByOwnerIdQuery query = new(project.OwnerId.Value);
            _mockProjectRepository.Setup(p => p.GetByOwnerIdAsync(project.OwnerId)).ReturnsAsync(project);

            //Act
            ProjectResponse? response = await _sut.Handle(query, CancellationToken.None);

            //Assert
            response.Should().NotBeNull();
            response!.Id.Should().Be(project.Id.Value);

        }


    }
}

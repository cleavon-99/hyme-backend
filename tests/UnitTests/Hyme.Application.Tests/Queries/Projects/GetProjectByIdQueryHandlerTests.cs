using AutoMapper;
using FluentAssertions;
using Hyme.Application.DTOs.Response;
using Hyme.Application.MappingProfiles;
using Hyme.Application.Queries.Projects;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;

namespace Hyme.Application.Tests.Queries.Projects
{
    public class GetProjectByIdQueryHandlerTests
    {

        private readonly Mock<IProjectRepository> _projectRepository;
        private readonly IMapper _mapper;

        public GetProjectByIdQueryHandlerTests()
        {
            _projectRepository = new();
            MapperConfiguration mapperConfiguration = new(options => 
            {
                options.AddProfile<ProjectMappingProfiles>();
            });

            _mapper = mapperConfiguration.CreateMapper();
        }

        [Fact]
        public async Task Handle_ShouldInvokeRepository_GetByIdAsync()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            ProjectId projectId = new(id);
            GetProjectByIdQuery query = new(id);
            var sut = new GetProjectByIdQueryHandler(_projectRepository.Object, _mapper);
            //Act
            ProjectResponse? project = await sut.Handle(query, CancellationToken.None);

            //Assert
            _projectRepository.Verify(p => p.GetByIdAsync(projectId));
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            GetProjectByIdQuery query = new(id);
            var sut = new GetProjectByIdQueryHandler(_projectRepository.Object, _mapper);
            //Act
            ProjectResponse? project = await sut.Handle(query, CancellationToken.None);

            //Assert
            project.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturnProjectResponse_WhenRepositoryReturnsAValue()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            ProjectId projectId = new(id);
            GetProjectByIdQuery query = new(id);
            _projectRepository.Setup(s => s.GetByIdAsync(projectId)).ReturnsAsync(Project.Create(projectId, new UserId(id)));
            var sut = new GetProjectByIdQueryHandler(_projectRepository.Object, _mapper);
            //Act
            ProjectResponse? project = await sut.Handle(query, CancellationToken.None);

            //Assert
            project.Should().NotBeNull();
            project!.Id.Should().Be(id);
        }
    }
}

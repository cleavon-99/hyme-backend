using AutoMapper;
using FluentAssertions;
using Hyme.Application.MappingProfiles;
using Hyme.Application.Queries.Projects;
using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using TestUtilities.Repository;

namespace Hyme.Application.Tests.Queries.Projects
{
    public class GetProjectsQueryHandlerTests
    {

        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly IMapper _mapper;
        private readonly GetProjectsQueryHandler _sut;

        public GetProjectsQueryHandlerTests()
        {
            _mockProjectRepository = new();
            MapperConfiguration mapperConfiguration = new(options => {
                options.AddProfile<ProjectMappingProfiles>();
            });
            _mapper = mapperConfiguration.CreateMapper();
            _sut = new(_mockProjectRepository.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldQueryRepositoryGetListAsync()
        {
            //Arrange
            GetProjectsQuery query = new(1, 20);
        
            //Act
            var result = await _sut.Handle(query, CancellationToken.None);

            //Assert
            _mockProjectRepository.Verify(p => p.GetListAsync(It.IsAny<PaginationFilter>()));
        }

        [Fact]
        public async Task Handle_ShouldReturnExactCollectionLength_WithPageNumberAndPageSize()
        {
            //Arrange
            GetProjectsQuery query = new(1, 20);
            List<Project> projects = ProjectRepositoryUtilities.CreateProjects();
            _mockProjectRepository.Setup(s => s.GetListAsync(It.IsAny<PaginationFilter>())).ReturnsAsync(projects);
            GetProjectsQueryHandler sut = new(_mockProjectRepository.Object, _mapper);

            //Act
            var result = await sut.Handle(query, CancellationToken.None);

            //Assert
            result.Data.Count.Should().Be(projects.Count);
            result.PageNumber.Should().Be(query.PageNumber);
            result.PageSize.Should().Be(query.PageSize);
        }


    }
}

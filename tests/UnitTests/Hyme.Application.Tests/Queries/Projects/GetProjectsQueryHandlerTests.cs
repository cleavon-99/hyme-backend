using AutoMapper;
using FluentAssertions;
using Hyme.Application.MappingProfiles;
using Hyme.Application.Queries.Projects;
using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;

namespace Hyme.Application.Tests.Queries.Projects
{
    public class GetProjectsQueryHandlerTests
    {

        private readonly Mock<IProjectRepository> _projectRepository;
        private readonly IMapper _mapper;

        public GetProjectsQueryHandlerTests()
        {
            _projectRepository = new();
            MapperConfiguration mapperConfiguration = new(options => {
                options.AddProfile<ProjectMappingProfiles>();
            });
            _mapper = mapperConfiguration.CreateMapper();
        }

        [Fact]
        public async Task Handle_ShouldQueryRepositoryGetListAsync()
        {
            //Arrange
            GetProjectsQuery query = new(1, 20);
            PaginationFilter filter = PaginationFilter.Create(query.PageNumber, query.PageSize);
            GetProjectsQueryHandler sut = new(_projectRepository.Object, _mapper);
            
            //Act
            var result = await sut.Handle(query, CancellationToken.None);

            //Assert
            _projectRepository.Verify(p => p.GetListAsync(It.IsAny<PaginationFilter>()));
        }

        [Fact]
        public async Task Handle_ShouldReturnExactCollectionLength_WithPageNumberAndPageSize()
        {
            //Arrange
            GetProjectsQuery query = new(1, 20);
            
            PaginationFilter filter = PaginationFilter.Create(query.PageNumber, query.PageSize);
            List<Project> projects = new() {
                Project.Create(new ProjectId(Guid.NewGuid()), new UserId(Guid.NewGuid())),
                Project.Create(new ProjectId(Guid.NewGuid()), new UserId(Guid.NewGuid())),
                Project.Create(new ProjectId(Guid.NewGuid()), new UserId(Guid.NewGuid()))
            };
            _projectRepository.Setup(s => s.GetListAsync(It.IsAny<PaginationFilter>())).ReturnsAsync(projects);
            GetProjectsQueryHandler sut = new(_projectRepository.Object, _mapper);

            //Act
            var result = await sut.Handle(query, CancellationToken.None);

            //Assert
            result.Data.Count.Should().Be(projects.Count);
            result.PageNumber.Should().Be(query.PageNumber);
            result.PageSize.Should().Be(query.PageSize);
        }


    }
}

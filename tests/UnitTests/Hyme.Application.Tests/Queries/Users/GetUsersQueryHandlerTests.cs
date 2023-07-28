using AutoMapper;
using FluentAssertions;
using Hyme.Application.DTOs.Response;
using Hyme.Application.MappingProfiles;
using Hyme.Application.Queries.Users;
using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.Repositories;
using Moq;
using TestUtilities.Repository;

namespace Hyme.Application.Tests.Queries.Users
{
    public class GetUsersQueryHandlerTests
    {

        private readonly IMapper _mapper;
        private readonly Mock<IUserRepository> _userRepository;

        public GetUsersQueryHandlerTests()
        {
            MapperConfiguration mapperConfiguration = new(options =>
            {
                options.AddProfile<UserMappingProfiles>();
            });

            _mapper = mapperConfiguration.CreateMapper();
            _userRepository = new();
        }

        [Fact]
        public async Task Handle_ShouldInvokeRepositoryGetListAsync()
        {
            //Arrange
            GetUsersQuery query = new(1, 20);
 
            List<User> users = UserRepositoryUtilities.GetUsers();
            _userRepository.Setup(u => u.GetListAsync(It.IsAny<PaginationFilter>())).ReturnsAsync(users);
            GetUsersQueryHandler sut = new(_userRepository.Object, _mapper);

            //Act
            var result = await sut.Handle(query, CancellationToken.None);

            //Assert
            _userRepository.Verify(u => u.GetListAsync(It.IsAny<PaginationFilter>()));
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedResponseWith3ItemsInTheData()
        {
            //Arrange
            GetUsersQuery query = new(1, 20);
            PaginationFilter filter = PaginationFilter.Create(query.PageNumber, query.PageSize);
            List<User> users = UserRepositoryUtilities.GetUsers();

            _userRepository.Setup(u => u.GetListAsync(It.IsAny<PaginationFilter>())).ReturnsAsync(users);
            GetUsersQueryHandler sut = new(_userRepository.Object, _mapper);

            //Act
            PagedResponse<UserResponse> result = await sut.Handle(query, CancellationToken.None);

            //Assert
            result.Data.Count.Should().Be(users.Count);
            result.PageNumber.Should().Be(query.PageNumber);
            result.PageSize.Should().Be(query.PageSize);
        }
    }
}

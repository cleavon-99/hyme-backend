using AutoMapper;
using FluentAssertions;
using Hyme.Application.DTOs.Response;
using Hyme.Application.MappingProfiles;
using Hyme.Application.Queries.Users;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using TestUtilities.Repository;

namespace Hyme.Application.Tests.Queries.Users
{
    public class GetUserByIdQueryHandlerTests
    {


        private readonly IMapper _mapper;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly GetUserByIdQueryHandler _sut;

        public GetUserByIdQueryHandlerTests()
        {
            MapperConfiguration mapperConfiguration = new(options =>
            {
                options.AddProfile<UserMappingProfiles>();
            });
            _mapper = mapperConfiguration.CreateMapper();
            _mockUserRepository = new();

            _sut = new(_mockUserRepository.Object, _mapper);

        }


        [Fact]
        public async Task Handle_ShouldInvokeRepositoryGetByidAsync()
        {
            //Arrange
            User user = UserRepositoryUtilities.GetUser();
            GetUserByIdQuery query = new(user.Id.Value);
            GetUserByIdQueryHandler sut = new(_mockUserRepository.Object, _mapper);

            //Act
            var result = await sut.Handle(query, CancellationToken.None);

            //Assert
            _mockUserRepository.Verify(u => u.GetByIdAsync(user.Id));
        }

        [Fact]
        public async Task Handle_ShoulReturnNull_WhenRepositoryReturnsNullValue()
        {
            //Arrange
            User user = UserRepositoryUtilities.GetUser();
            GetUserByIdQuery query = new(user.Id.Value);
            GetUserByIdQueryHandler sut = new(_mockUserRepository.Object, _mapper);

            //Act
            UserResponse? result = await sut.Handle(query, CancellationToken.None);

            //Assert      
            result.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturUserResponse_WhenRepositoryReturnsNonNullValue()
        {
            //Arrange
            User user = UserRepositoryUtilities.GetUser();
            GetUserByIdQuery query = new(user.Id.Value);
            _mockUserRepository.Setup(u => u.GetByIdAsync(user.Id)).ReturnsAsync(user);
            GetUserByIdQueryHandler sut = new(_mockUserRepository.Object, _mapper);

            //Act
            UserResponse? result = await sut.Handle(query, CancellationToken.None);

            //Assert      
            result.Should().NotBeNull();
            result!.Id.Should().Be(user.Id.Value);
            result!.WalletAddress.Should().Be(user.WalletAddress.Value);
            result!.Name.Should().Be(user.Name);
        }
    }
}

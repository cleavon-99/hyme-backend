using AutoMapper;
using FluentAssertions;
using Hyme.Application.DTOs.Response;
using Hyme.Application.MappingProfiles;
using Hyme.Application.Queries.UserProfiles;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Application.Tests.Queries.UserProfiles
{
    public class GetUserProfileByIdQueryHandlerTests
    {


        private readonly IMapper _mapper;
        private readonly Mock<IUserRepository> _userRepository;

        public GetUserProfileByIdQueryHandlerTests()
        {
            MapperConfiguration mapperConfiguration = new(options => 
            { 
                options.AddProfile<UserMappingProfiles>();  
            });
            _mapper = mapperConfiguration.CreateMapper();

            _userRepository = new();
        }


        [Fact]
        public async Task Handle_ShouldInvokeRepositoryGetByidAsync()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            UserId userId = new(id);
            GetUserProfleByIdQuery query = new(id);
            GetUserProfileByIdQueryHandler sut = new(_userRepository.Object, _mapper);

            //Act
            var result = await sut.Handle(query, CancellationToken.None);

            //Assert
            _userRepository.Verify(u => u.GetByIdAsync(userId));
        }

        [Fact]
        public async Task Handle_ShoulReturnNull_WhenRepositoryReturnsNullValue()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            GetUserProfleByIdQuery query = new(id);
            GetUserProfileByIdQueryHandler sut = new(_userRepository.Object, _mapper);

            //Act
            UserResponse? result = await sut.Handle(query, CancellationToken.None);

            //Assert      
            result.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturUserResponse_WhenRepositoryReturnsNonNullValue()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            UserId userId = new(id);
            User user = User.Create(userId, new WalletAddress("0x00"));
            _userRepository.Setup(u => u.GetByIdAsync(userId)).ReturnsAsync(user);
            GetUserProfleByIdQuery query = new(id);
            GetUserProfileByIdQueryHandler sut = new(_userRepository.Object, _mapper);

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

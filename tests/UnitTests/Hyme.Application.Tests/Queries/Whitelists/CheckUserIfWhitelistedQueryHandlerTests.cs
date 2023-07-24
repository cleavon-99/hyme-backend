using FluentAssertions;
using Hyme.Application.Queries.Whitelists;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using TestUtilities.Queries;
using TestUtilities.Repository;


namespace Hyme.Application.Tests.Queries.Whitelists
{
    public class CheckUserIfWhitelistedQueryHandlerTests
    {

        private readonly Mock<IWhitelistRepository> _mockWhiteListRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly CheckUserIfWhitelistedQueryHandler _sut;

        public CheckUserIfWhitelistedQueryHandlerTests()
        {
            _mockWhiteListRepository = new();
            _mockUserRepository = new();
            _sut = new(_mockUserRepository.Object, _mockWhiteListRepository.Object);
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryGetByIdAsync()
        {
            //Arrange
            CheckUserIfWhitelistedQuery query = WhitelistUtitlities.CheckUserIfWhitelistedQuery();

            //Act
            var result = await _sut.Handle(query, CancellationToken.None);

            //Assert
            _mockUserRepository.Verify(u => u.GetByIdAsync(new UserId(query.Id)));
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenUserRepositoryReturnsNull()
        {
            //Arrange
            CheckUserIfWhitelistedQuery query = WhitelistUtitlities.CheckUserIfWhitelistedQuery();

            //Act
            var result = await _sut.Handle(query, CancellationToken.None);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ShouldCallWhiteListRepository_WhenUserRepositoryReturnsAValue()
        {
            //Arrange
            CheckUserIfWhitelistedQuery query = WhitelistUtitlities.CheckUserIfWhitelistedQuery();
            User user = UserRepositoryUtilities.GetUser();
            _mockUserRepository.Setup(u => u.GetByIdAsync(new UserId(query.Id)))
                .ReturnsAsync(user);

            //Act
            var result = await _sut.Handle(query, CancellationToken.None);

            //Assert
            _mockWhiteListRepository.Verify(w => w.FindAsync(user.WalletAddress));
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenWhiteListRepositoryReturnsNull()
        {
            //Arrange
            CheckUserIfWhitelistedQuery query = WhitelistUtitlities.CheckUserIfWhitelistedQuery();
            User user = UserRepositoryUtilities.GetUser();
            _mockUserRepository.Setup(u => u.GetByIdAsync(new UserId(query.Id)))
                .ReturnsAsync(user);

            //Act
            var result = await _sut.Handle(query, CancellationToken.None);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ShouldReturnFalseTrue_WhenWhiteListRepositoryReturnsAValue()
        {
            //Arrange
            CheckUserIfWhitelistedQuery query = WhitelistUtitlities.CheckUserIfWhitelistedQuery();
            User user = UserRepositoryUtilities.GetUser();
            _mockUserRepository.Setup(u => u.GetByIdAsync(new UserId(query.Id)))
                .ReturnsAsync(user);
            _mockWhiteListRepository.Setup(u => u.FindAsync(user.WalletAddress))
                .ReturnsAsync(new Whitelist(user.WalletAddress));

            //Act
            var result = await _sut.Handle(query, CancellationToken.None);

            //Assert
            result.Should().BeTrue();
        }
    }
}

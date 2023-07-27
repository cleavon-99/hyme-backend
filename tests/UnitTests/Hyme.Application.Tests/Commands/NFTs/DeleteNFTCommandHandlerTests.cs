using FluentAssertions;
using Hyme.Application.Commands.NFTs;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using FluentResults.Extensions.FluentAssertions;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using TestUtilities.Repository;

namespace Hyme.Application.Tests.Commands.NFTs
{
    public class DeleteNFTCommandHandlerTests
    {


        private readonly Mock<INFTRepository> _mockNFTRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly DeleteNFTCommandHandler _sut;

        public DeleteNFTCommandHandlerTests()
        {
            _mockNFTRepository = new();
            _mockUnitOfWork = new();
            _sut = new(_mockNFTRepository.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldInvoke_RepositoryGetByIdAsync()
        {
            //Arrange
            DeleteNFTCommand command = new(Guid.NewGuid());

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockNFTRepository.Verify(n => n.GetByIdAsync(new NFTId(command.NFTId)));
        }

        [Fact]
        public async Task Handle_ShouldReturnNFTNotFoundErrorResult_WhenRepositoryReturnsNullValue()
        {
            //Arrange
            DeleteNFTCommand command = new(Guid.NewGuid());

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            result.Should().HaveReason(new NFTNotFoundError(command.NFTId));
        }

        [Fact]
        public async Task Handle_ShouldDeleteNFT_WhenRepositoryReturnsAValue()
        {
            //Arrange   
            NFT nft = NFTRepositoryUtilities.CreateNFT();
            DeleteNFTCommand command = new(nft.Id.Value);
            _mockNFTRepository.Setup(n => n.GetByIdAsync(nft.Id)).ReturnsAsync(nft);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            nft.DateDeleted.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_WhenRepositoryReturnsAValue()
        {
            //Arrange   
            NFT nft = NFTRepositoryUtilities.CreateNFT();
            DeleteNFTCommand command = new(nft.Id.Value);
            _mockNFTRepository.Setup(n => n.GetByIdAsync(nft.Id)).ReturnsAsync(nft);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenRepositoryReturnsAValue()
        {
            //Arrange   
            NFT nft = NFTRepositoryUtilities.CreateNFT();
            DeleteNFTCommand command = new(nft.Id.Value);
            _mockNFTRepository.Setup(n => n.GetByIdAsync(nft.Id)).ReturnsAsync(nft);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeSuccess();
        }
    }
}

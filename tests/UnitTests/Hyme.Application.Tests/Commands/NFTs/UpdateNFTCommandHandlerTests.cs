using Hyme.Application.Commands.NFTs;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using FluentResults.Extensions.FluentAssertions;
using Moq;
using System.Diagnostics.CodeAnalysis;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using TestUtilities.Repository;
using FluentAssertions;

namespace Hyme.Application.Tests.Commands.NFTs
{
    public class UpdateNFTCommandHandlerTests 
    {

        private readonly Mock<INFTRepository> _mockNFTRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UpdateNFTCommandHandler _sut;

        public UpdateNFTCommandHandlerTests()
        {
            _mockNFTRepository = new();
            _mockUnitOfWork = new();
            _sut = new(_mockNFTRepository.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldInvoke_NFTRepositoryGetByIdAsync()
        {
            //Arrange
            UpdateNFTCommand command = new(Guid.NewGuid(), "New Title", "New Description");
            

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockNFTRepository.Verify(n => n.GetByIdAsync(new NFTId(command.NFTId)));
        }

        [Fact]
        public async Task Handle_ShouldReturnNFTNotFoundErrorResult_WhenRepositoryReturnsNullValue()
        {
            //Arrange
            UpdateNFTCommand command = new(Guid.NewGuid(), "New Title", "New Description");


            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            result.Should().HaveReason(new NFTNotFoundError(command.NFTId));
        }

        [Fact]
        public async Task Handle_ShouldUpdateNft_WhenRepositoryReturnsAValue()
        {
            //Arrange
            NFT nft = NFTRepositoryUtilities.CreateNFT();
            UpdateNFTCommand command = new(nft.Id.Value, "New Title", "New Description");
            _mockNFTRepository.Setup(n => n.GetByIdAsync(nft.Id)).ReturnsAsync(nft);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            nft.Title.Should().Be(command.Title);
            nft.Description.Should().Be(command.Description);
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_WhenRepositoryReturnsAValue()
        {
            //Arrange
            NFT nft = NFTRepositoryUtilities.CreateNFT();
            UpdateNFTCommand command = new(nft.Id.Value, "New Title", "New Description");
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
            UpdateNFTCommand command = new(nft.Id.Value, "New Title", "New Description");
            _mockNFTRepository.Setup(n => n.GetByIdAsync(nft.Id)).ReturnsAsync(nft);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeSuccess();
        }



    }
}

using AutoMapper;
using FluentAssertions;
using Hyme.Application.MappingProfiles;
using Hyme.Application.Queries.NFTs;
using Hyme.Domain.Entities;
using Hyme.Domain.Primitives;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;

namespace Hyme.Application.Tests.Queries.NFTs
{
    public class GetProjectNFTsQueryHandlerTests
    {

        private readonly Mock<INFTRepository> _mockNFTRepository;
        private readonly IMapper _mapper;
        private readonly GetProjectNFTsQueryHandler _sut;

        public GetProjectNFTsQueryHandlerTests()
        {
            _mockNFTRepository = new();
            MapperConfiguration mapperConfiguration = new(options => {
                options.AddProfile<NFTMappingProfiles>();
            });

            _mapper = mapperConfiguration.CreateMapper();
            _sut = new(_mockNFTRepository.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldInvoke_NFTRepositoryGetNFTsAsync()
        {
            //Arrange
            GetProjectNFTsQuery query = new(Guid.NewGuid(), 1, 20);

            //Act
            var result = await _sut.Handle(query, CancellationToken.None);

            //Assert
            _mockNFTRepository.Verify(n => n.GetNFTsAsync(new ProjectId(query.ProjectId), It.IsAny<PaginationFilter>()));
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedResponse_OfNFT()
        {
            //Arrange
            GetProjectNFTsQuery query = new(Guid.NewGuid(), 1, 20);
            List<NFT> nfts = new() { new(new NFTId(Guid.NewGuid()), new ProjectId(Guid.NewGuid()), "title", "description", "image.jpeg") };
            _mockNFTRepository.Setup(n => n.GetNFTsAsync(new ProjectId(query.ProjectId), It.IsAny<PaginationFilter>())).ReturnsAsync(nfts);
            //Acts
            var result = await _sut.Handle(query, CancellationToken.None);

            //Assert
            result.Data.Count.Should().Be(nfts.Count);
            result.PageNumber.Should().Be(query.PageNumber);
            result.PageSize.Should().Be(query.PageSize);
        }

    }
    
}

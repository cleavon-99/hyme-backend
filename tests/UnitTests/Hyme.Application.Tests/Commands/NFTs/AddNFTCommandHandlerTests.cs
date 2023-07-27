using AutoMapper;
using FluentAssertions;
using Hyme.Application.MappingProfiles;
using Hyme.Application.Services;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;
using FluentResults.Extensions.FluentAssertions;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using TestUtilities.Repository;
using Hyme.Application.Commands.NFTs;

namespace Hyme.Application.Tests.Commands.NFTs
{
    public class AddNFTCommandHandlerTests
    {

        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly AddNFTCommandHandler _sut;
        private readonly IMapper _mapper;

        public AddNFTCommandHandlerTests()
        {
            MapperConfiguration mapperConfiguration = new(options =>
            {
                options.AddProfile<NFTMappingProfiles>();
            });
            _mapper = mapperConfiguration.CreateMapper();
            _mockProjectRepository = new();
            _mockUnitOfWork = new();
            _mockBlobService = new();
            _sut = new AddNFTCommandHandler(_mockProjectRepository.Object, _mockUnitOfWork.Object, _mockBlobService.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldCallProjectRepository_GetByIdAsync()
        {
            //Arrange
            AddNFTCommand command = new AddNFTCommand(Guid.NewGuid(), "Title", "Description", "Image.jpeg", Array.Empty<byte>());
            //Act

            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockProjectRepository.Verify(p => p.GetByIdAsync(new ProjectId(command.ProjectId)));
        }

        [Fact]
        public async Task Handle_ShouldReturnProjectNotFoundErrorResult_WhenProjectRepositoryReturnsNull()
        {
            //Arrange
            AddNFTCommand command = new AddNFTCommand(Guid.NewGuid(), "Title", "Description", "Image.jpeg", Array.Empty<byte>());
            //Act

            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeFailure();
            result.Should().HaveReason(new ProjectNotFoundError(command.ProjectId));
        }

        [Fact]
        public async Task Handle_ShouldAddNFTToTheProject_WhenRepositoryReturnsAValue()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            AddNFTCommand command = new AddNFTCommand(project.Id.Value, "Title", "Description", "Image.jpeg", Array.Empty<byte>());
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act

            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            project.NFTs.Count().Should().Be(1);
        }

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_WhenRepositoryReturnsAValue()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            AddNFTCommand command = new AddNFTCommand(project.Id.Value, "Title", "Description", "Image.jpeg", Array.Empty<byte>());
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act

            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldUploadImageToBlobStorage_WhenRepositoryReturnsAValue()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            AddNFTCommand command = new AddNFTCommand(project.Id.Value, "Title", "Description", "Image.jpeg", Array.Empty<byte>());
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act

            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _mockBlobService.Verify(m => m.UploadImageAsync(command.Image, command.FileName));
        }

        [Fact]
        public async Task Handle_ShouldReturnNFTResponse_WhenRepositoryReturnsAValue()
        {
            //Arrange
            Project project = ProjectRepositoryUtilities.CreateProject();
            AddNFTCommand command = new AddNFTCommand(project.Id.Value, "Title", "Description", "Image.jpeg", Array.Empty<byte>());
            _mockProjectRepository.Setup(p => p.GetByIdAsync(project.Id)).ReturnsAsync(project);

            //Act

            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Value.Title.Should().Be(command.Title);
            result.Value.Description.Should().Be(command.Description);
        }
    }
}

using AutoMapper;
using AutoMapper.Configuration.Annotations;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using Hyme.Application.Commands.Projects;
using Hyme.Application.Errors;
using Hyme.Application.MappingProfiles;
using Hyme.Domain.Entities;
using Hyme.Domain.Errors;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using Moq;

namespace Hyme.Application.Tests.Commands.Projects
{
    public class AddProjectCommandHandlerTests
    {

        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly IMapper _mapper;
        
        public AddProjectCommandHandlerTests()
        {
            _userRepository = new();
            _unitOfWork = new();
            MapperConfiguration mapperConfiguration = new(options =>
            {
                options.AddProfile<ProjectMappingProfiles>();
            });
            _mapper = mapperConfiguration.CreateMapper();
        }

        [Fact]
        public async Task Handle_ShouldCallRepository_GetUserByIdAsync()
        {
            //Arrange
            AddProjectCommand command = new(Guid.NewGuid(), "Title", "Short Description", "Project Description");
            AddProjectCommandHandler sut = new(_userRepository.Object, _unitOfWork.Object, _mapper);

            //Act
            await sut.Handle(command, CancellationToken.None);

            //Assert
            _userRepository.Verify(u => u.GetByIdWithProjectAsync(new UserId(command.UserId)));
        }

        [Fact]
        public async Task Handle_ShouldReturnUserNotFoundError_WhenRepositoryReturnsNullUser()
        {
            //Arrange
            AddProjectCommand command = new(Guid.NewGuid(), "Title", "Short Description", "Project Description");
            AddProjectCommandHandler sut = new(_userRepository.Object, _unitOfWork.Object, _mapper);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().HaveReason(new UserNotFoundError(command.UserId));
        }

        [Fact]
        public async Task Handle_ShouldAddProjectToUser_WhenRepositoryReturnsAUser()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            UserId userId = new(id);
            WalletAddress walletAddress = new("0x00");
            User user = User.Create(userId, walletAddress);

            _userRepository.Setup(u => u.GetByIdWithProjectAsync(It.IsAny<UserId>())).ReturnsAsync(user);
            AddProjectCommand command = new(Guid.NewGuid(), "Title", "Short Description", "Project Description");
            AddProjectCommandHandler sut = new(_userRepository.Object, _unitOfWork.Object, _mapper);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            user.Project.Should().NotBeNull();
        }

        //[Fact]
        //public async Task Handle_ShouldReturnProjectAlreadeCreatedError_WhenUserHasAProject()
        //{
        //    //Arrange
        //    Guid id = Guid.NewGuid();
        //    UserId userId = new(id);
        //    WalletAddress walletAddress = new("0x00");
        //    User user = User.Create(userId, walletAddress);

        //    _userRepository.Setup(u => u.GetByIdWithProjectAsync(It.IsAny<UserId>())).ReturnsAsync(user);
        //    AddProjectCommand command = new(Guid.NewGuid(), "Title", "Short Description", "Project Description");
        //    AddProjectCommandHandler sut = new(_userRepository.Object, _unitOfWork.Object, _mapper);

        //    //Act
        //    var result = await sut.Handle(command, CancellationToken.None);

        //    //Assert
        //    result.Should().HaveReason(new ProjectAlreadyCreatedError());
        //}

        [Fact]
        public async Task Handle_ShouldInvokeUnitOfWorkSaveChangesAsync_WhenUserDoesNotHaveAProject()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            UserId userId = new(id);
            WalletAddress walletAddress = new("0x00");
            User user = User.Create(userId, walletAddress);


            _userRepository.Setup(u => u.GetByIdWithProjectAsync(It.IsAny<UserId>())).ReturnsAsync(user);
            AddProjectCommand command = new(Guid.NewGuid(), "Title", "Short Description", "Project Description");
            AddProjectCommandHandler sut = new(_userRepository.Object, _unitOfWork.Object, _mapper);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_AfterSaveChanges()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            UserId userId = new(id);
            WalletAddress walletAddress = new("0x00");
            User user = User.Create(userId, walletAddress);


            _userRepository.Setup(u => u.GetByIdWithProjectAsync(It.IsAny<UserId>())).ReturnsAsync(user);
            AddProjectCommand command = new(Guid.NewGuid(), "Title", "Short Description", "Project Description");
            AddProjectCommandHandler sut = new(_userRepository.Object, _unitOfWork.Object, _mapper);

            //Act
            var result = await sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeSuccess();
        }
    }
}

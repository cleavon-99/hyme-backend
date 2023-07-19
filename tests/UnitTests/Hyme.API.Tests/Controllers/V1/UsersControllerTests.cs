using FluentAssertions;
using FluentResults;
using Hyme.API.Controllers.V1;
using Hyme.Application.Commands.UserProfiles;
using Hyme.Application.DTOs.Request;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Queries.UserProfiles;
using Hyme.Shared.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Hyme.API.Tests.Controllers.V1
{
    public class UsersControllerTests
    {

        private readonly Mock<ISender> _sender;

        public UsersControllerTests()
        {
            _sender = new();    
        }

        [Fact]
        public async Task GetUsers_ShouldReturnBadRequestObjectResult_WhenPageNumberOrPageSize_IsLessThan1()
        {
            //Arrange
            PaginationRequest request = new() { PageNumber = 0, PageSize = 0 };
            var sut = new UsersController(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();
            
            //Act
            var result = await sut.GetUsers(request);
            
            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetUsers_ShouldSendGetUsersQuery()
        {
            //Arrange
            PaginationRequest request = new();
            GetUsersQuery query = new(request.PageNumber, request.PageSize);          
            var sut = new UsersController(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();
            _sender.Setup(s => s.Send(query, sut.HttpContext.RequestAborted));

            //Act
            var result = await sut.GetUsers(request);

            //Assert
            _sender.Verify(s => s.Send(query, sut.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOkObjectResult_WhenSenderReturnsAValue()
        {
            //Arrange
            PaginationRequest request = new();
            GetUsersQuery query = new(request.PageNumber, request.PageSize);
            var sut = new UsersController(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();
            _sender.Setup(s => s.Send(query, sut.HttpContext.RequestAborted));

            //Act
            var result = await sut.GetUsers(request);

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }


        [Fact]
        public async Task GetMyProfile_ShouldReturn401Unauthorize_WhenNoIdFoundInTheClaims()
        {
            //Arrange
            var sut = new UsersController(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();

            //Act
            var result = await sut.GetMyProfile();

            //Assert
            result.Result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task GetMyProfile_ShoudSendGetMyProfileQuery_WhenUserIdClaimsIsPresent()
        {
            //Arrange
            string userId = Guid.NewGuid().ToString();
            GetUserProfleByIdQuery query = new(Guid.Parse(userId));
            ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[] 
            { 
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));
            
            var sut = new UsersController(_sender.Object);
            
            sut.ControllerContext.HttpContext = new DefaultHttpContext() 
            {
                User = user
            };
            
            _sender.Setup(s => s.Send(query, sut.HttpContext.RequestAborted));

            //Act
            var result = await sut.GetMyProfile();

            //Assert
            _sender.Verify(s => s.Send(query, sut.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task GetMyProfile_ShouldReturnNotFoundResult_WhenQueryReturnsNullValue()
        {
            //Arrange
            string userId = Guid.NewGuid().ToString();
            GetUserProfleByIdQuery query = new(Guid.Parse(userId));
            ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));
            var sut = new UsersController(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = user
            };
            _sender.Setup(s => s.Send(query, sut.HttpContext.RequestAborted)).ReturnsAsync(() => null);

            //Act
            var result = await sut.GetMyProfile();

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetMyProfile_ShouldReturnOkObjectResult_WhenQueryReturnsNonNullValue()
        {
            //Arrange
            string userId = Guid.NewGuid().ToString();
            GetUserProfleByIdQuery query = new(Guid.Parse(userId));
            ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));
            var sut = new UsersController(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = user
            };

            _sender.Setup(s => s.Send(query, sut.HttpContext.RequestAborted)).ReturnsAsync(new UserResponse());

            //Act
            var result = await sut.GetMyProfile();

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }


        [Fact]
        public async Task UpdateProfile_ShouldReturn401Unauthorized_WhenClaimsIdIsNull()
        {
            //Arrange
            UpdateUserProfileRequest request = new() { Name = "Arjay" };
            var sut = new UsersController(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();

            //Act
            var result = await sut.UpdateProfile(request);

            //Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task UpdateProfile_ShouldSendUpdateProfileCommand_WhenUserIdClaimIsPresent()
        {
            //Arrange
            Guid userId = Guid.NewGuid();
            
            UpdateUserProfileRequest request = new() { Name = "Arjay" };
            UpdateUserProfileCommand command = new(userId, request.Name);
            
            ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));
            var sut = new UsersController(_sender.Object);     
            sut.ControllerContext.HttpContext = new DefaultHttpContext() { User = user};
            _sender.Setup(s => s.Send(command, sut.HttpContext.RequestAborted)).ReturnsAsync(Result.Fail(new UserNotFoundError(userId)));
            //Act
            var result = await sut.UpdateProfile(request);

            //Assert
            _sender.Verify(s => s.Send(command, sut.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnNotFound_WhenCommandRetunsFailureResult()
        {
            //Arrange
            Guid userId = Guid.NewGuid();

            UpdateUserProfileRequest request = new() { Name = "Arjay" };
            UpdateUserProfileCommand command = new(userId, request.Name);

            ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));
            var sut = new UsersController(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
            _sender.Setup(s => s.Send(command, sut.HttpContext.RequestAborted)).ReturnsAsync(Result.Fail(new UserNotFoundError(userId)));
            //Act
            var result = await sut.UpdateProfile(request);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpdateProfile_ShoulrReturn204NoContent_WhenCommandReturnsSuccessResult()
        {
            //Arrange
            Guid userId = Guid.NewGuid();

            UpdateUserProfileRequest request = new() { Name = "Arjay" };
            UpdateUserProfileCommand command = new(userId, request.Name);

            ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));
            var sut = new UsersController(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
            _sender.Setup(s => s.Send(command, sut.HttpContext.RequestAborted)).ReturnsAsync(Result.Ok());
            //Act
            var result = await sut.UpdateProfile(request);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}

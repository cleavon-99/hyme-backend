using FluentAssertions;
using FluentResults;
using Hyme.API.Controllers.V1;
using Hyme.Application.Commands.Users;
using Hyme.Application.DTOs.Request;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Errors;
using Hyme.Application.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TestUtilities.Constants;

namespace Hyme.API.Tests.Controllers.V1
{
    public class UsersControllerTests
    {

        private readonly Mock<ISender> _sender;
        private readonly UsersController _sut;

        public UsersControllerTests()
        {
            _sender = new();
            _sut = new UsersController(_sender.Object);
            _sut.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new(new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, Constants.User.UserId.Value.ToString())
                }))
            };
        }

        [Fact]
        public async Task GetUsers_ShouldReturnBadRequestObjectResult_WhenPageNumberOrPageSize_IsLessThan1()
        {
            //Arrange
            PaginationRequest request = new() { PageNumber = 0, PageSize = 0 };
            
            //Act
            var result = await _sut.GetUsers(request);
            
            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetUsers_ShouldSendGetUsersQuery()
        {
            //Arrange
            PaginationRequest request = new();
            GetUsersQuery query = new(request.PageNumber, request.PageSize);          
            _sender.Setup(s => s.Send(query, _sut.HttpContext.RequestAborted));

            //Act
            var result = await _sut.GetUsers(request);

            //Assert
            _sender.Verify(s => s.Send(query, _sut.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOkObjectResult_WhenSenderReturnsAValue()
        {
            //Arrange
            PaginationRequest request = new();
            GetUsersQuery query = new(request.PageNumber, request.PageSize);
           
            _sut.ControllerContext.HttpContext = new DefaultHttpContext();
            _sender.Setup(s => s.Send(query, _sut.HttpContext.RequestAborted));

            //Act
            var result = await _sut.GetUsers(request);

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }


        [Fact]
        public async Task GetMyProfile_ShoudSendGetMyProfileQuery_WhenUserIdClaimsIsPresent()
        {
            //Arrange
            GetUserByIdQuery query = new(Constants.User.UserId.Value);

            _sender.Setup(s => s.Send(query, _sut.HttpContext.RequestAborted));

            //Act
            var result = await _sut.GetMyProfile();

            //Assert
            _sender.Verify(s => s.Send(query, _sut.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task GetMyProfile_ShouldReturnNotFoundResult_WhenQueryReturnsNullValue()
        {
            //Arrange
            GetUserByIdQuery query = new(Constants.User.UserId.Value);
          
            _sender.Setup(s => s.Send(query, _sut.HttpContext.RequestAborted)).ReturnsAsync(() => null);

            //Act
            var result = await _sut.GetMyProfile();

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetMyProfile_ShouldReturnOkObjectResult_WhenQueryReturnsNonNullValue()
        {
            //Arrange
            GetUserByIdQuery query = new(Constants.User.UserId.Value);


            _sender.Setup(s => s.Send(query, _sut.HttpContext.RequestAborted)).ReturnsAsync(new UserResponse());

            //Act
            var result = await _sut.GetMyProfile();

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }


        [Fact]
        public async Task UpdateProfile_ShouldReturn401Unauthorized_WhenClaimsIdIsNull()
        {
            //Arrange
            _sut.ControllerContext.HttpContext = new DefaultHttpContext();
            UpdateUserRequest request = new() { Name = Constants.User.Name };
           
            //Act
            var result = await _sut.UpdateProfile(request);

            //Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task UpdateProfile_ShouldSendUpdateProfileCommand_WhenUserIdClaimIsPresent()
        {
            //Arrange
            
            UpdateUserRequest request = new() { Name = Constants.User.Name };
            UpdateUserCommand command = new(Constants.User.UserId.Value, request.Name);

          
            _sender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted))
                .ReturnsAsync(Result.Fail(new UserNotFoundError(command.UserId)));
            //Act
            var result = await _sut.UpdateProfile(request);

            //Assert
            _sender.Verify(s => s.Send(command, _sut.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnNotFound_WhenCommandRetunsFailureResult()
        {
            //Arrange
            UpdateUserRequest request = new() { Name = Constants.User.Name };
            UpdateUserCommand command = new(Constants.User.UserId.Value, request.Name);

            _sender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted))
                .ReturnsAsync(Result.Fail(new UserNotFoundError(command.UserId)));
            //Act
            var result = await _sut.UpdateProfile(request);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpdateProfile_ShoulrReturn204NoContent_WhenCommandReturnsSuccessResult()
        {
            //Arrange
            UpdateUserRequest request = new() { Name = Constants.User.Name };
            UpdateUserCommand command = new(Constants.User.UserId.Value, request.Name);


            _sender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted)).ReturnsAsync(Result.Ok());
            //Act
            var result = await _sut.UpdateProfile(request);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}

using FluentAssertions;
using Hyme.API.Controllers.V1;
using Hyme.Application.Queries.Whitelists;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TestUtilities.Constants;

namespace Hyme.API.Tests.Controllers.V1
{
    public class WhitelistsControllerTests
    {
        private readonly Mock<ISender> _mockSender;
        private readonly WhitelistsController _sut;

        public WhitelistsControllerTests()
        {
            _mockSender = new();
            _sut = new(_mockSender.Object);
            _sut.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new(new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier, Constants.User.UserId.Value.ToString())
                }))
            };
        }


        [Fact]
        public async Task CheckIfWhitelisted_ShouldSendCheckUserIfWhitelistedQuery()
        {
            //Arrange
        

            //Act
            var result = await _sut.CheckIfWhitelisted();

            //Assert
            _mockSender.Verify(s => s.Send(new CheckUserIfWhitelistedQuery(Constants.User.UserId.Value), CancellationToken.None));
        }

        [Fact]
        public async Task CheckIfWhitelisted_ShouldReturnOkResult_WhenQueryReturnsTrue()
        {
            //Arrange
           
            CheckUserIfWhitelistedQuery query = new(Constants.User.UserId.Value);
            _mockSender.Setup(s => s.Send(query, _sut.HttpContext.RequestAborted)).ReturnsAsync(true);
            //Act
            var result = await _sut.CheckIfWhitelisted();

            //Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task CheckIfWhitelisted_ShouldReturnNotFound_WhenQueryReturnsFalse()
        {
            //Arrange

            CheckUserIfWhitelistedQuery query = new(Constants.User.UserId.Value);
            _mockSender.Setup(s => s.Send(query, _sut.HttpContext.RequestAborted)).ReturnsAsync(false);
            //Act
            var result = await _sut.CheckIfWhitelisted();

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}

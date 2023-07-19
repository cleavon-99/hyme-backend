using FluentAssertions;
using FluentResults;
using Hyme.API.Controllers.V1;
using Hyme.Application.Commands.Authentication;
using Hyme.Application.DTOs.Response;
using Hyme.Shared.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hyme.API.Tests.Controllers.V1
{
    public class AuthenticationControllerTests
    {

        private readonly Mock<ISender> _sender;
        private readonly ConnectToWalletCommand _command;
        public AuthenticationControllerTests()
        {
            _sender = new();
            _command = new("HYME-SECRET-SIGN-MESSAGE", "0x750ec05cc380129c6b03b4881a392a861fd046d33d148bbba6181bad87fec42c0825548a4bb72c70dd877d8870637f5f29842ebb096121da0ce7069f482b1b311b", "0xb9c102eE5c79B3063724C4Ddd15b30d1Ab9901aC");
        }


        [Fact]
        public async Task ConnectToWallet_SendConnectToWalletCommand()
        {
            //Arrange   
            var sut = new AuthenticationController(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();
            _sender.Setup(s => s.Send(_command, sut.HttpContext.RequestAborted)).ReturnsAsync(Result.Fail(new InvalidWalletError()));
            
            //Act
            await sut.ConnectToWallet(_command);

            //Assert
            _sender.Verify(s => s.Send(_command, sut.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task ConnectToWallet_ShouldReturnBadRequestObjectResult_WhenResultReturnsFailure()
        {
            //Arrange
            var sut = new AuthenticationController(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();
            _sender.Setup(s => s.Send(_command, sut.HttpContext.RequestAborted)).ReturnsAsync(Result.Fail(new InvalidWalletError()));

            //Act
            var result = await sut.ConnectToWallet(_command);

            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();           
        }

        [Fact]
        public async Task ConnectToWallet_ShouldReturnOkObjectResult_WhenResultReturnsSuccess()
        {
            //Arrange
            var sut = new AuthenticationController(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();
            _sender.Setup(s => s.Send(_command, sut.HttpContext.RequestAborted)).ReturnsAsync(Result.Ok(It.IsAny<AuthenticationResponse>()));
            
            //Act
            var result = await sut.ConnectToWallet(_command);
            
            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }
    }
}

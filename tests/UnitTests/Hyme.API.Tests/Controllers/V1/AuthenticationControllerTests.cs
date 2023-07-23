using FluentAssertions;
using FluentResults;
using Hyme.API.Controllers.V1;
using Hyme.Application.Commands.Authentication;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestUtilities.Commands;

namespace Hyme.API.Tests.Controllers.V1
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<ISender> _mockSender;
        private readonly AuthenticationController _sut;

        public AuthenticationControllerTests()
        {
            _mockSender = new();
            _sut = new(_mockSender.Object);
            _sut.ControllerContext.HttpContext = new DefaultHttpContext();
        }


        [Fact]
        public async Task ConnectToWallet_SendConnectToWalletCommand()
        {
            //Arrange   
            ConnectToWalletCommand command = AuthenticationUtilities.ConnectToWalletCommand();
            
            _mockSender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted))
                .ReturnsAsync(Result.Fail(new InvalidWalletError()));
            
            //Act
            await _sut.ConnectToWallet(command);

            //Assert
            _mockSender.Verify(s => s.Send(command, _sut.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task ConnectToWallet_ShouldReturnBadRequestObjectResult_WhenResultReturnsFailure()
        {
            //Arrange
            ConnectToWalletCommand command = AuthenticationUtilities.ConnectToWalletCommand();
            _mockSender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted))
                .ReturnsAsync(Result.Fail(new InvalidWalletError()));

            //Act
            var result = await _sut.ConnectToWallet(command);

            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();           
        }

        [Fact]
        public async Task ConnectToWallet_ShouldReturnOkObjectResult_WhenResultReturnsSuccess()
        {
            //Arrange
            ConnectToWalletCommand command = AuthenticationUtilities.ConnectToWalletCommand();
            _mockSender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted))
                .ReturnsAsync(Result.Ok(It.IsAny<AuthenticationResponse>()));
            
            //Act
            var result = await _sut.ConnectToWallet(command);
            
            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

       
    }
}

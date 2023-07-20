using FluentAssertions;
using Hyme.API.Controllers.V1;
using Hyme.Application.DTOs.Request;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Queries.Projects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.API.Tests.Controllers.V1
{
    public class ProjectsControllerTests
    {

        private readonly Mock<ISender> _sender;

        public ProjectsControllerTests()
        {
            _sender = new();
        }

        [Fact]
        public async Task GetProjects_ShouldReturnBadRequesObjectResult_WhenPageNumberOrPageSizeIsLessThan1()
        {
            //Arrange
            PaginationRequest request = new() { PageNumber = 0, PageSize = 0};
            ProjectsController sut = new(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();


            //Act
            var result = await sut.GetProjects(request);

            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetProjects_ShouldSendGetProjectsQuery_WhenPaginationRequestIsValid()
        {
            //Arrange
            PaginationRequest request = new();
            ProjectsController sut = new(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();


            //Act
            var result = await sut.GetProjects(request);

            //Assert
            _sender.Verify(s => s.Send(new GetProjectsQuery(request.PageNumber, request.PageSize), sut.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task GetProjects_ShouldReturnOkObjectResult_AfterSendingQuery()
        {
            //Arrange
            PaginationRequest request = new();
            ProjectsController sut = new(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();


            //Act
            var result = await sut.GetProjects(request);

            //Assert
            _sender.Verify(s => s.Send(new GetProjectsQuery(request.PageNumber, request.PageSize), sut.HttpContext.RequestAborted));
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetProjectById_ShouldSendGetProjectByIdQuery()
        {
            //Arrange
            Guid id = Guid.NewGuid();

            ProjectsController sut = new(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();

            //Act
            var result = await sut.GetProjectById(id);

            //Assert
            _sender.Verify(s => s.Send(new GetProjectByIdQuery(id), sut.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task GetProjectById_ShouldReturnNotFound_WhenQueryReturnsNull()
        {
            //Arrange
            Guid id = Guid.NewGuid();

            ProjectsController sut = new(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();

            //Act
            var result = await sut.GetProjectById(id);

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetProjectById_ShouldReturnOkObjectResult_WhenQueryReturnsAValue()
        {
            //Arrange
            Guid id = Guid.NewGuid();

            GetProjectByIdQuery query = new(id);
            
            ProjectsController sut = new(_sender.Object);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();
            _sender.Setup(s => s.Send(query, sut.HttpContext.RequestAborted)).ReturnsAsync(new ProjectResponse() { Id = id});
            //Act
            var result = await sut.GetProjectById(id);

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result.Result!;
            ProjectResponse response = (ProjectResponse)okResult.Value!;
            response.Id.Should().Be(id);
        }
    }
}

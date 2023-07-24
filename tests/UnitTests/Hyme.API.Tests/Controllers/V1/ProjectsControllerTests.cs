﻿using FluentAssertions;
using FluentResults;
using Hyme.API.Controllers.V1;
using Hyme.Application.Commands.Projects;
using Hyme.Application.DTOs.Request;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Errors;
using Hyme.Application.Queries.Projects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestUtilities.Commands;
using TestUtilities.Constants;
using TestUtilities.Queries;

namespace Hyme.API.Tests.Controllers.V1
{
    public class ProjectsControllerTests
    {

        private readonly Mock<ISender> _sender;
        private readonly ProjectsController _sut;
        public ProjectsControllerTests()
        {
            _sender = new();
            _sut = new(_sender.Object);
            _sut.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task GetProjects_ShouldReturnBadRequesObjectResult_WhenPageNumberOrPageSizeIsLessThan1()
        {
            //Arrange
            PaginationRequest request = new() { PageNumber = 0, PageSize = 0};

            //Act
            var result = await _sut.GetProjects(request);

            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetProjects_ShouldSendGetProjectsQuery_WhenPaginationRequestIsValid()
        {
            //Arrange
            PaginationRequest request = new();

            //Act
            var result = await _sut.GetProjects(request);

            //Assert
            _sender.Verify(s => s.Send(new GetProjectsQuery(request.PageNumber, request.PageSize), _sut.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task GetProjects_ShouldReturnOkObjectResult_AfterSendingQuery()
        {
            //Arrange
            PaginationRequest request = new();
          
            //Act
            var result = await _sut.GetProjects(request);

            //Assert
            _sender.Verify(s => s.Send(new GetProjectsQuery(request.PageNumber, request.PageSize), _sut.HttpContext.RequestAborted));
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetProjectById_ShouldSendGetProjectByIdQuery()
        {
            //Arrange
            GetProjectByIdQuery query = ProjectQueriesUtilities.GetProjectByIdQuery();
   
            //Act
            var result = await _sut.GetProjectById(Constants.Project.ProjectId.Value);

            //Assert
            _sender.Verify(s => s.Send(query, _sut.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task GetProjectById_ShouldReturnNotFound_WhenQueryReturnsNull()
        {
            //Arrange
            GetProjectByIdQuery query = ProjectQueriesUtilities.GetProjectByIdQuery();

            //Act
            var result = await _sut.GetProjectById(query.Id);

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetProjectById_ShouldReturnOkObjectResult_WhenQueryReturnsAValue()
        {
            //Arrange 
            GetProjectByIdQuery query = ProjectQueriesUtilities.GetProjectByIdQuery();
            _sender.Setup(s => s.Send(query, _sut.HttpContext.RequestAborted)).ReturnsAsync(new ProjectResponse() { Id = query .Id});
            
            //Act
            var result = await _sut.GetProjectById(query.Id);

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result.Result!;
            ProjectResponse response = (ProjectResponse)okResult.Value!;
            response.Id.Should().Be(query.Id);
        }



        //[Fact]
        //public async Task AddProject_ShouldReturn401UnAuthorized_ClaimIdIsNull()
        //{
        //    //Arrange
        //    ProjectRequest request = new() 
        //    {
        //        Title = "Test Title",
        //        ShortDescription = "Short Description",
        //        ProjectDescription = "Project Description"
        //    };

        //    //Act
        //    var result = await _sut.AddProject(request);

        //    //Assert
        //    result.Result.Should().BeOfType<UnauthorizedResult>();
        //}

        //[Fact]
        //public async Task AddProject_ShouldSend_AddProjectCommand_WhenIdOfUserIsPresent()
        //{
        //    //Arrange
        //    ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, _id.ToString())
        //    }));

        //    ProjectRequest request = new()
        //    {
        //        Title = "Test Title",
        //        ShortDescription = "Short Description",
        //        ProjectDescription = "Project Description"
        //    };

        //    ProjectResponse response = new()
        //    {
        //        Id = _id,
        //        Banner = "",
        //        DateCreated = DateTime.Now,
        //        Logo = "",
        //        ProjectDescription = request.ProjectDescription,
        //        ShortDescription = request.ShortDescription
        //    };

        //    _sut.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
        //    AddProjectCommand command = new(_id, request.Title, request.ShortDescription, request.ProjectDescription);
        //    _sender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted)).ReturnsAsync(Result.Ok(response));

        //    //Act
        //    var result = await _sut.AddProject(request);

        //    //Assert
        //    _sender.Verify(s => s.Send(new AddProjectCommand(_id, request.Title, request.ShortDescription, request.ProjectDescription), _sut.HttpContext.RequestAborted));
        //}

        //[Fact]
        //public async Task AddProject_ShouldReturnNotFound_WhenCommandReturns_UserNotFoundError()
        //{
        //    //Arrange
            
        //    ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, _id.ToString())
        //    }));

            
        //    ProjectRequest request = new()
        //    {
        //        Title = "Test Title",
        //        ShortDescription = "Short Description",
        //        ProjectDescription = "Project Description"
        //    };
            
      
        //    _sut.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
        //    AddProjectCommand command = new AddProjectCommand(_id, request.Title, request.ShortDescription, request.ProjectDescription);
        //    _sender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted)).ReturnsAsync(Result.Fail(new UserNotFoundError(_id)));
        //    //Act
        //    var result = await _sut.AddProject(request);

        //    //Assert
        //    result.Result.Should().BeOfType<NotFoundResult>();
        //}

        //[Fact]
        //public async Task AddProject_ShouldReturnCreatedAtRoute_WhenResultIsSuccess()
        //{
        //    //Arrange
            
        //    ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, _id.ToString())
        //    }));


        //    ProjectRequest request = new()
        //    {
        //        Title = "Test Title",
        //        ShortDescription = "Short Description",
        //        ProjectDescription = "Project Description"
        //    };

        //    ProjectResponse response = new()
        //    {
        //        Id = _id,
        //        Banner = "",
        //        DateCreated = DateTime.Now,
        //        Logo = "",
        //        ProjectDescription = request.ProjectDescription,
        //        ShortDescription = request.ShortDescription
        //    };

        //    _sut.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
        //    AddProjectCommand command = new(_id, request.Title, request.ShortDescription, request.ProjectDescription);
        //    _sender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted)).ReturnsAsync(Result.Ok(response));
        //    //Act
        //    var result = await _sut.AddProject(request);

        //    //Assert
        //    result.Result.Should().BeOfType<CreatedAtRouteResult>();
        //}

        [Fact]
        public async Task ApproveProject_ShouldSend_ApproveProjectCommand()
        {
            //Arrange 
            ApproveProjectCommand command = ProjectCommandsUtilities.ApproveProjectCommand();       
            _sender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted))
                .ReturnsAsync(Result.Fail(new ProjectNotFoundError(command.ProjectId)));
            
            //Act
            var result = await _sut.ApproveProject(command.ProjectId);

            //Assert
            _sender.Verify(s => s.Send(command, _sut.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task ApproveProject_ShouldReturnNotFound_WhenCommandReturnsProjectNotFoundError()
        {
            //Arrange 
            ApproveProjectCommand command = ProjectCommandsUtilities.ApproveProjectCommand();
            _sender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted))
                .ReturnsAsync(Result.Fail(new ProjectNotFoundError(command.ProjectId)));
            
            //Act
            var result = await _sut.ApproveProject(command.ProjectId);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ApproveProject_ShouldReturnNoContentResult_WhenCommandReturnsSuccessResult()
        {
            //Arrange      
            ApproveProjectCommand command = ProjectCommandsUtilities.ApproveProjectCommand();
            _sender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted)).ReturnsAsync(Result.Ok());
            
            //Act
            var result = await _sut.ApproveProject(command.ProjectId);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RejectProject_ShouldSendRejectProjectCommand()
        {
            //Arrange
            RejectProjectCommand command = ProjectCommandsUtilities.RejectProjectCommand();
            _sender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted))
                .ReturnsAsync(Result.Fail(new ProjectNotFoundError(command.ProjectId)));
            
            //Act
            var result = await _sut.RejectProject(command.ProjectId);

            //Assert
            _sender.Verify(s => s.Send(command, _sut.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task RejectProject_ShouldReturnNotFound_WhenCommandReturnsProjectNotFoundError()
        {
            //Arrange
            RejectProjectCommand command = ProjectCommandsUtilities.RejectProjectCommand();
            _sender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted))
                .ReturnsAsync(Result.Fail(new ProjectNotFoundError(command.ProjectId)));
            
            //Act
            var result = await _sut.RejectProject(command.ProjectId);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task RejectProject_ShouldReturnNoContentResult_WhenCommandReturnsSuccessResult()
        {
            //Arrange
            RejectProjectCommand command = ProjectCommandsUtilities.RejectProjectCommand();
            _sender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted)).ReturnsAsync(Result.Ok);
            
            //Act
            var result = await _sut.RejectProject(command.ProjectId);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }


        [Fact]
        public async Task UpdateGeneralInfo_ShouldSend_UpdateProjectGeneralInfoCommand()
        {
            //Arrange
            UpdateProjectInfoCommand command = ProjectCommandsUtilities.UpdateProjectInfoCommand();
            _sender.Setup(s => s.Send(command, _sut.HttpContext.RequestAborted)).ReturnsAsync(Result.Ok);

            //Act
            var result = await _sut.UpdateGeneralInfo(command.ProjectId, new ProjectRequest() 
            { 
                Title = command.Title, 
                ShortDescription = command.ShortDescription, 
                ProjectDescription = command.ProjectDescription
            });

            //Assert
            _sender.Verify(s => s.Send(command, CancellationToken.None));    
        }

        [Fact]
        public async Task UpdateGeneralInfo_ShouldReturnNotFoundResult_WhenCommandHasProjectNotFoundError()
        {
            //Arrange
            UpdateProjectInfoCommand command = ProjectCommandsUtilities.UpdateProjectInfoCommand();

            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Fail(new ProjectNotFoundError(command.ProjectId)));

            //Act
            var result = await _sut.UpdateGeneralInfo(command.ProjectId, new ProjectRequest()
            {
                Title = command.Title,
                ShortDescription = command.ShortDescription,
                ProjectDescription = command.ProjectDescription
            });

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpdateGeneralInfo_ShouldReturnNoContent_WhenCommandReturnsSuccessResult()
        {
            //Arrange
            UpdateProjectInfoCommand command = ProjectCommandsUtilities.UpdateProjectInfoCommand();

            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Ok());

            //Act
            var result = await _sut.UpdateGeneralInfo(command.ProjectId, new ProjectRequest()
            {
                Title = command.Title,
                ShortDescription = command.ShortDescription,
                ProjectDescription = command.ProjectDescription
            });

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}

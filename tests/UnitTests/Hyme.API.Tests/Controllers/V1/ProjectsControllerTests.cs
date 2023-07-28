using FluentAssertions;
using FluentResults;
using Hyme.API.Controllers.V1;
using Hyme.Application.Commands.NFTs;
using Hyme.Application.Commands.Projects;
using Hyme.Application.DTOs.Request;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Errors;
using Hyme.Application.Queries.NFTs;
using Hyme.Application.Queries.Projects;
using Hyme.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Text;
using TestUtilities.Commands;
using TestUtilities.Constants;
using TestUtilities.Queries;
using static TestUtilities.Constants.Constants;

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

        [Fact]
        public async Task UpdateProjectLogo_ShouldCallMediatorUpdateProjectLogoCommand()
        {
            //Arrange
            UpdateProjectLogoCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "Filename");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "image.jpeg");
            _sender.Setup(s => s.Send(It.IsAny<UpdateProjectLogoCommand>(), CancellationToken.None)).ReturnsAsync(Result.Ok());
            //Act
            var result = await _sut.UpdateProjectLogo(command.ProjectId, file);

            //Assert
            _sender.Verify(s => s.Send(It.IsAny<UpdateProjectLogoCommand>(), _sut.HttpContext.RequestAborted));
        }


        [Fact]
        public async Task UpdateProjectLogo_ShouldReturnNotFound_WhenCommandReturnsProjectNotFoundError()
        {
            //Arrange
            UpdateProjectLogoCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "Filename");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "image.jpeg");
            _sender.Setup(s => s.Send(It.IsAny<UpdateProjectLogoCommand>(), CancellationToken.None))
                .ReturnsAsync(Result.Fail(new ProjectNotFoundError(command.ProjectId)));
            //Act
            var result = await _sut.UpdateProjectLogo(command.ProjectId, file);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpdateProjectLogo_ShouldReturnNoContent_WhenCommandReturnsSuccess()
        {
            //Arrange
            UpdateProjectLogoCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "Filename");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "image.jpeg");
            _sender.Setup(s => s.Send(It.IsAny<UpdateProjectLogoCommand>(), CancellationToken.None))
                .ReturnsAsync(Result.Ok());
            
            //Act
            var result = await _sut.UpdateProjectLogo(command.ProjectId, file);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateProjectBanner_ShouldCall_UpdateProjectBannerCommand()
        {
            //Arrange
            UpdateProjectBannerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "Banner.png");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", command.FileName);
            _sender.Setup(s => s.Send(It.IsAny<UpdateProjectBannerCommand>(), CancellationToken.None))
                .ReturnsAsync(Result.Fail(new ProjectNotFoundError(command.ProjectId)));

            //Act
            var result = await _sut.UpdateProjectBanner(command.ProjectId, file);

            //Assert
            _sender.Verify(s => s.Send(It.IsAny<UpdateProjectBannerCommand>(), CancellationToken.None));
        }

        [Fact]
        public async Task UpdateProjectBanner_ShouldReturnNotFoundResult_WhenResultReturnsFailureResult()
        {
            //Arrange
            UpdateProjectBannerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "Banner.png");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", command.FileName);
            _sender.Setup(s => s.Send(It.IsAny<UpdateProjectBannerCommand>(), CancellationToken.None))
                .ReturnsAsync(Result.Fail(new ProjectNotFoundError(command.ProjectId)));

            //Act
            var result = await _sut.UpdateProjectBanner(command.ProjectId, file);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpdateProjectBanner_ShouldReturnNoContentResult_WhenResultReturnsSuccessResult()
        {
            //Arrange
            UpdateProjectBannerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "Banner.png");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", command.FileName);
            _sender.Setup(s => s.Send(It.IsAny<UpdateProjectBannerCommand>(), CancellationToken.None))
                .ReturnsAsync(Result.Ok());

            //Act
            var result = await _sut.UpdateProjectBanner(command.ProjectId, file);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateProjecTrailer_ShouldSendUpdateProjectCommand()
        {
            //Arrange
            UpdateProjectTrailerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "Banner.png");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", command.FileName);
            _sender.Setup(s => s.Send(It.IsAny<UpdateProjectTrailerCommand>(), CancellationToken.None))
                .ReturnsAsync(Result.Ok());

            //Act
            var result = await _sut.UpdateProjectTrailer(command.ProjectId, file);

            //Assert
            _sender.Verify(s => s.Send(It.IsAny<UpdateProjectTrailerCommand>(), CancellationToken.None));
        }

        [Fact]
        public async Task UpdateProjecTrailer_ShouldReturnNotFound_WhenUpdateCommandReturnsProjectNotFoundResult()
        {
            //Arrange
            UpdateProjectTrailerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "Banner.png");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", command.FileName);
            _sender.Setup(s => s.Send(It.IsAny<UpdateProjectTrailerCommand>(), CancellationToken.None))
                .ReturnsAsync(Result.Fail(new ProjectNotFoundError(command.ProjectId)));

            //Act
            var result = await _sut.UpdateProjectTrailer(command.ProjectId, file);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpdateProjecTrailer_ShouldReturnNoContentResult_WhenUpdateCommandReturnsSuccessResult()
        {
            //Arrange
            UpdateProjectTrailerCommand command = new(Guid.NewGuid(), Array.Empty<byte>(), "Banner.png");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", command.FileName);
            _sender.Setup(s => s.Send(It.IsAny<UpdateProjectTrailerCommand>(), CancellationToken.None))
                .ReturnsAsync(Result.Ok());

            //Act
            var result = await _sut.UpdateProjectTrailer(command.ProjectId, file);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetMyProject_ShouldReturn401UnAuthorize_WhenNoCurrentlyLoggedInUser()
        {
            //Arrange
            //Act
            var result = await _sut.GetMyProject();

            //Assert
            result.Result.Should().BeOfType<UnauthorizedResult>();
        }


        [Fact]
        public async Task GetMyProject_ShouldSendGetProjectByOwnerIdQuery_WhenThereIsCurrentlyLoggedInUser()
        {
            //Arrange
            Guid id = User.UserId.Value;
            GetProjectByOwnerIdQuery query = new(id);

            _sut.ControllerContext.HttpContext = new DefaultHttpContext() 
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] 
                { 
                    new Claim(ClaimTypes.NameIdentifier, query.Id.ToString())
                }))
            };

            //Act
            var result = await _sut.GetMyProject();

            //Assert
            _sender.Verify(s => s.Send(query, CancellationToken.None));
        }

        [Fact]
        public async Task GetMyProject_ShouldReturnNotFoundResult_WhenQueryReturnsNull()
        {
            //Arrange
            Guid id = User.UserId.Value;
            GetProjectByOwnerIdQuery query = new(id);

            _sut.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, query.Id.ToString())
                }))
            };

            _sender.Setup(s => s.Send(query, CancellationToken.None)).ReturnsAsync(() => null);

            //Act
            var result = await _sut.GetMyProject();

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetMyProject_ShouldReturnOkObjectResult_WhenQueryReturnsAValue()
        {
            //Arrange
            Guid id = User.UserId.Value;
            GetProjectByOwnerIdQuery query = new(id);
            ProjectResponse response = new() { Id = id };
            _sut.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, query.Id.ToString())
                }))
            };

            _sender.Setup(s => s.Send(query, CancellationToken.None)).ReturnsAsync(response);

            //Act
            var result = await _sut.GetMyProject();

            //Assert
            OkObjectResult okObjectResult = (OkObjectResult)result.Result!;
            okObjectResult.Value.Should().BeOfType<ProjectResponse>();
        }

        [Fact]
        public async Task AddNFT_ShouldSendAddNFTCommand()
        {
            //Arrange
            AddNFTCommand command = new(Guid.NewGuid(), "Title", "Description", "add.png", Array.Empty<byte>());
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", command.FileName);
            _sender.Setup(s => s.Send(It.IsAny<AddNFTCommand>(), CancellationToken.None)).ReturnsAsync(Result.Fail(new ProjectNotFoundError(command.ProjectId)));
            NFTRequest request = new()
            {
                Title = command.Title,
                Description = command.Description,
                Image = file
            };
            //Act
            var result = await _sut.AddNFT(command.ProjectId, request);

            //Assert
            _sender.Verify(s => s.Send(It.IsAny<AddNFTCommand>(), CancellationToken.None));
        }

        [Fact]
        public async Task AddNFT_ShouldReturnNotFound_WhenSenderReturnsFailureResult()
        {
            //Arrange
            AddNFTCommand command = new(Guid.NewGuid(), "Title", "Description", "add.png", Array.Empty<byte>());
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", command.FileName);
            NFTRequest request = new()
            {
                Title = command.Title,
                Description = command.Description,
                Image = file
            };

            _sender.Setup(s => s.Send(It.IsAny<AddNFTCommand>(), CancellationToken.None)).ReturnsAsync(Result.Fail(new ProjectNotFoundError(command.ProjectId)));

            //Act
            var result = await _sut.AddNFT(command.ProjectId, request);

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AddNFT_ShouldReturnOkObjectResult_WhenSenderReturnsSuccessResult()
        {
            //Arrange
            AddNFTCommand command = new(Guid.NewGuid(), "Title", "Description", "add.png", Array.Empty<byte>());
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", command.FileName);
            NFTRequest request = new()
            {
                Title = command.Title,
                Description = command.Description,
                Image = file
            };

            _sender.Setup(s => s.Send(It.IsAny<AddNFTCommand>(), CancellationToken.None)).ReturnsAsync(Result.Ok());

            //Act
            var result = await _sut.AddNFT(command.ProjectId, request);

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }


        [Fact]
        public async Task GetNFTs_ShouldReturnBadRequestObjectResult_WhenPageNumberOrPageSizeIsLessThan1()
        {
            //Arrange
            GetProjectNFTsQuery query = new(Guid.NewGuid(), 0, 0);

            //Act
            var result = await _sut.GetNfts(query.ProjectId, new PaginationRequest() { PageNumber = query.PageNumber, PageSize = query.PageSize });

            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetNFTs_ShouldSend_GetNFTsQuery()
        {
            //Arrange
            GetProjectNFTsQuery query = new(Guid.NewGuid(), 1, 20);

            //Act
            var result = await _sut.GetNfts(query.ProjectId, new PaginationRequest() {PageNumber = query.PageNumber, PageSize = query.PageSize });

            //Assert
            _sender.Verify(s => s.Send(query, CancellationToken.None));
        }

        [Fact]
        public async Task GetNFTs_ShouldReturnOkObjectResult_WhenPageNumberOrPageSizeIsValid()
        {
            //Arrange
            GetProjectNFTsQuery query = new(Guid.NewGuid(), 1, 20);

            //Act
            var result = await _sut.GetNfts(query.ProjectId, new PaginationRequest() { PageNumber = query.PageNumber, PageSize = query.PageSize });

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }


        [Fact]
        public async Task PublishMyProject_ShouldReturn401Unauthorized_WhenNoIdFoundInClaims()
        {
            //Arrange
            //Act
            var result = await _sut.PublishMyProject();

            //Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task PublishMyProject_ShouldSendPublishMyprojectCommand()
        {
            //Arrange
            UserId userId = Constants.User.UserId;
            _sut.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())
                }))
            };

            PublishProjectCommand command = new(userId.Value);
            _sender.Setup(s => s.Send(command, CancellationToken.None))
               .ReturnsAsync(Result.Fail(new ProjectNotFoundError(userId.Value)));

            //Act
            var result = await _sut.PublishMyProject();

            //Assert
            _sender.Verify(s => s.Send(command, CancellationToken.None));
        }

        [Fact]
        public async Task PublishMyProject_ShouldReturnNotFound_WhenCommandReturnsFailureResult()
        {
            //Arrange
            UserId userId = Constants.User.UserId;
            _sut.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())
                }))
            };

            PublishProjectCommand command = new(userId.Value);
            _sender.Setup(s => s.Send(command, CancellationToken.None))
                .ReturnsAsync(Result.Fail(new ProjectNotFoundError(userId.Value)));

            //Act
            var result = await _sut.PublishMyProject();

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task PublishMyProject_ShouldReturnNoContentResult_WhenCommandReturnsSuccessResult()
        {
            //Arrange
            UserId userId = Constants.User.UserId;
            _sut.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())
                }))
            };

            PublishProjectCommand command = new(userId.Value);
            _sender.Setup(s => s.Send(command, CancellationToken.None))
                .ReturnsAsync(Result.Ok());

            //Act
            var result = await _sut.PublishMyProject();

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteNFT_ShouldSendDeleteNFTCommand()
        {

            //Arrage
            DeleteNFTCommand command = new(Guid.NewGuid());
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Fail(new NFTNotFoundError(command.NFTId)));


            //Act
            var result = await _sut.DeleteNFT(command.NFTId);

            //Assert
            _sender.Verify(s => s.Send(command, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteNFT_ShouldReturnNotFound_WhenCommandReturnsFailureResult()
        {

            //Arrage
            DeleteNFTCommand command = new(Guid.NewGuid());
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Fail(new NFTNotFoundError(command.NFTId)));

            //Act
            var result = await _sut.DeleteNFT(command.NFTId);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteNFT_ShouldReturnNoContentResult_WhenCommandReturnsSuccessResult()
        {

            //Arrage
            DeleteNFTCommand command = new(Guid.NewGuid());
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Ok());

            //Act
            var result = await _sut.DeleteNFT(command.NFTId);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateNFT_ShouldSend_UpdateNFTCommand()
        {
            //Arrange
            Guid nftId = Guid.NewGuid();
            UpdateNFTCommand command = new(nftId, "Title", "Description");
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Ok());

            //Act
            await _sut.UpdateNFT(nftId, new UpdateNFTRequest() { Title = command.Title, Description = command.Description});

            //Assert
            _sender.Verify(n => n.Send(command, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateNFT_ShouldReturnNotFound_WhenCommandReturnsFailureResult()
        {
            //Arrange
            Guid nftId = Guid.NewGuid();
            UpdateNFTCommand command = new(nftId, "Title", "Description");
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Fail(new NFTNotFoundError(command.NFTId)));

            //Act
            var result = await _sut.UpdateNFT(nftId, new UpdateNFTRequest() { Title = command.Title, Description = command.Description });

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpdateNFT_ShouldReturnNoContentResult_WhenCommandReturnsSuccessResult()
        {
            //Arrange
            Guid nftId = Guid.NewGuid();
            UpdateNFTCommand command = new(nftId, "Title", "Description");
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Ok());

            //Act
            var result = await _sut.UpdateNFT(nftId, new UpdateNFTRequest() { Title = command.Title, Description = command.Description });

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteLogo_ShouldSendDeleteLogoCommand()
        {
            //Arrange
            Guid projectd = Guid.NewGuid();
            DeleteLogoCommand command = new(projectd);
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Fail(""));

            //Act
            var result = await _sut.DeleteLogo(projectd);

            //Assert
            _sender.Verify(s => s.Send(command, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteLogo_ShouldReturnNotFound_WhenCommandReturnsFailureResult()
        {
            //Arrange
            Guid projectd = Guid.NewGuid();
            DeleteLogoCommand command = new(projectd);
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Fail(""));

            //Act
            var result = await _sut.DeleteLogo(projectd);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteLogo_ShouldReturnNoContentResult_WhenCommandReturnsSuccessResult()
        {
            //Arrange
            Guid projectd = Guid.NewGuid();
            DeleteLogoCommand command = new(projectd);
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Ok());

            //Act
            var result = await _sut.DeleteLogo(projectd);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteBanner_ShouldSendDeleteBannerCommand()
        {
            //Arrange
            Guid projectd = Guid.NewGuid();
            DeleteBannerCommand command = new(projectd);
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Fail(""));

            //Act
            var result = await _sut.DeleteBanner(projectd);

            //Assert
            _sender.Verify(s => s.Send(command, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteBanner_ShouldReturnNotFound_WhenCommandReturnsFailureResult()
        {
            //Arrange
            Guid projectd = Guid.NewGuid();
            DeleteBannerCommand command = new(projectd);
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Fail(""));

            //Act
            var result = await _sut.DeleteBanner(projectd);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteBanner_ShouldReturnNoContentResult_WhenCommandReturnsSuccessResult()
        {
            //Arrange
            Guid projectd = Guid.NewGuid();
            DeleteBannerCommand command = new(projectd);
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Ok());

            //Act
            var result = await _sut.DeleteBanner(projectd);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteTrailer_ShouldSendDeleteTrailerCommand()
        {
            //Arrange
            Guid projectd = Guid.NewGuid();
            DeleteTrailerCommand command = new(projectd);
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Fail(""));

            //Act
            var result = await _sut.DeleteTrailer(projectd);

            //Assert
            _sender.Verify(s => s.Send(command, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteTrailer_ShouldReturnNotFound_WhenCommandReturnsFailureResult()
        {
            //Arrange
            Guid projectd = Guid.NewGuid();
            DeleteTrailerCommand command = new(projectd);
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Fail(""));

            //Act
            var result = await _sut.DeleteTrailer(projectd);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteTrailer_ShouldReturnNoContentResult_WhenCommandReturnsSuccessResult()
        {
            //Arrange
            Guid projectd = Guid.NewGuid();
            DeleteTrailerCommand command = new(projectd);
            _sender.Setup(s => s.Send(command, CancellationToken.None)).ReturnsAsync(Result.Ok());

            //Act
            var result = await _sut.DeleteTrailer(projectd);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}

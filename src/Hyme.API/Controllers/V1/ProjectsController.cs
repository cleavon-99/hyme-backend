using Hyme.API.Extensions;
using Hyme.Application.Commands.Projects;
using Hyme.Application.DTOs.Request;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Errors;
using Hyme.Application.Queries.Projects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hyme.API.Controllers.V1
{

    /// <summary>
    /// 
    /// </summary>
    [Route("projects")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ApiBase
    {
        private readonly ISender _sender;

        /// <summary>
        /// 
        /// </summary>
        public ProjectsController(ISender sender)
        {
            _sender = sender;
        }


        /// <summary>
        /// List of projects
        /// </summary>
        /// <param name="paginationRequest">Page number and page size</param>
        /// <returns></returns>
        /// <response code="400">Page number or page size cannot be less than 1</response>
        /// <response code="200">Success</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ProjectResponse>>> GetProjects([FromQuery]PaginationRequest paginationRequest)
        {
            if (paginationRequest.PageSize < 1 || paginationRequest.PageNumber < 1)
                return BadRequest("Page number or page size cannot be less than 1");

            PagedResponse<ProjectResponse> projects = 
                await _sender.Send(new GetProjectsQuery(paginationRequest.PageNumber, paginationRequest.PageSize), HttpContext.RequestAborted);

            return Ok(projects);
        }


        /// <summary>
        /// Retrieve project by Id
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <returns></returns>
        /// <response code="404">Project not found</response>
        /// <response code="200">Success</response>
        [HttpGet("{id}", Name = "GetProjectById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectResponse>> GetProjectById(Guid id)
        {
            ProjectResponse? project = await _sender.Send(new GetProjectByIdQuery(id));
            if (project is null)
                return NotFound();
            return Ok(project);
        }

        /// <summary>
        /// Get my project
        /// </summary>
        /// <returns></returns>
        /// <response code="401">Not logged in</response>
        /// <response code="404">Not Found</response>
        /// <response code="200">Success</response>
        [HttpGet("myProject")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ProjectResponse>> GetMyProject()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();

            ProjectResponse? response = await _sender.Send(new GetProjectByOwnerIdQuery(Guid.Parse(userId)));
            if (response is null)
                return NotFound();
            return Ok(response);
        }



        ///// <summary>
        ///// Create Project
        ///// </summary>
        ///// <param name="projectRequest"></param>
        ///// <returns></returns>
        ///// <response code="401">Not loged in or user isn't registered</response>
        ///// <response code="404">User isn't registered</response>
        ///// <response code="201">Project successfully Created</response>
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<ProjectResponse>> AddProject([FromBody]ProjectRequest projectRequest)
        //{
        //    string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (userId is null)
        //        return Unauthorized();

        //    var result = await _sender.Send(
        //        new AddProjectCommand(
        //            Guid.Parse(userId), 
        //            projectRequest.Title, 
        //            projectRequest.ShortDescription, 
        //            projectRequest.ProjectDescription), 
        //        HttpContext.RequestAborted);

        //    if (result.IsFailed)
        //    {
        //        if (result.HasError<ValidationError>(out var errors))
        //            return Problem(errors);

        //        if (result.HasError<UserNotFoundError>())
        //            return NotFound();

        //        if(result.HasError<ProjectAlreadyCreatedError>())
        //            return BadRequest("Project is already Created");
        //    }
        //    return CreatedAtRoute(nameof(GetProjectById), new { result.Value.Id}, result.Value);
        //}


        /// <summary>
        /// Approve a project
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <returns></returns>
        /// <response code="404">Project not found</response>
        /// <response code="204">Success</response>
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin, Super Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ApproveProject(Guid id)
        {
            var result = await _sender.Send(new ApproveProjectCommand(id), HttpContext.RequestAborted);
            if(result.IsFailed)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Reject Project
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <returns></returns>
        /// <response code="204">Success</response>
        /// <response code="404">Project not found</response>
        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin, Super Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RejectProject(Guid id)
        {
            var result = await _sender.Send(new RejectProjectCommand(id), HttpContext.RequestAborted);
            if (result.IsFailed)
                return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Update general Info
        /// </summary>
        /// <param name="id">project id</param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="404">Project not found</response>
        /// <response code="204">Success</response>
        [HttpPut("{id}/general")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateGeneralInfo(Guid id, [FromBody] ProjectRequest request)
        {
            var result = await _sender.Send(new UpdateProjectInfoCommand(id, request.Title, request.ShortDescription, request.ProjectDescription), HttpContext.RequestAborted);
            if (result.IsFailed)
            {
                if (result.HasError<ProjectNotFoundError>())
                    return NotFound();

                if (result.HasError<ValidationError>(out var validationErrors))
                    return Problem(validationErrors);
            }
            return NoContent();
        }

        /// <summary>
        /// Update project logo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPut("{id}/logo")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateProjectLogo(Guid id, [FromForm]IFormFile image)
        {
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            byte[] logo = await image.ToByteArrayAsync();
            var result = await _sender.Send(new UpdateProjectLogoCommand(id, logo, fileName), HttpContext.RequestAborted);
            if (result.IsFailed)
                return NotFound();

            return NoContent();
        }


        /// <summary>
        /// Update project banner
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <param name="image">Banner Image</param>
        /// <returns></returns>
        /// <response code="404">Project not found</response>
        /// <response code="204">Success</response>
        [HttpPut("{id}/banner")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateProjectBanner(Guid id, [FromForm]IFormFile image)
        {
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            byte[] banner = await image.ToByteArrayAsync();
            var result = await _sender.Send(new UpdateProjectBannerCommand(id, banner, fileName), HttpContext.RequestAborted);
            if(result.IsFailed) 
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Update project trailer
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <param name="video">Video Trailer</param>
        /// <returns></returns>
        /// <response code="404">Project not found</response>
        /// <response code="204">Success</response>
        [HttpPut("{id}/trailer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateProjectTrailer(Guid id, [FromForm]IFormFile video)
        {
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(video.FileName)}";
            byte[] trailer = await video.ToByteArrayAsync();
            var result = await _sender.Send(new UpdateProjectTrailerCommand(id, trailer, fileName));
            if(result.IsFailed) 
                return NotFound();
            return NoContent();
        }


    }
}

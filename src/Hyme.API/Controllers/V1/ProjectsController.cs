﻿using Hyme.Application.Commands.Projects;
using Hyme.Application.DTOs.Request;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Errors;
using Hyme.Application.Queries.Projects;
using Hyme.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace Hyme.API.Controllers.V1
{

    /// <summary>
    /// 
    /// </summary>
    [Route("projects")]
    [ApiController]
    public class ProjectsController : ControllerBase
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
        /// Create Project
        /// </summary>
        /// <param name="projectRequest"></param>
        /// <returns></returns>
        /// <response code="401">Not loged in or user isn't registered</response>
        /// <response code="404">User isn't registered</response>
        /// <response code="201">Project successfully Created</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ProjectResponse>> AddProject([FromBody]ProjectRequest projectRequest)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();

            var result = await _sender.Send(
                new AddProjectCommand(
                    Guid.Parse(userId), 
                    projectRequest.Title, 
                    projectRequest.ShortDescription, 
                    projectRequest.ProjectDescription), 
                HttpContext.RequestAborted);

            if (result.IsFailed)
            {
                if (result.HasError<UserNotFoundError>())
                    return NotFound();

                if(result.HasError(out IEnumerable<ProjectAlreadyCreatedError> errors))
                    return BadRequest(errors.FirstOrDefault()!.Message);
            }

            return CreatedAtRoute(nameof(GetProjectById), new { result.Value.Id}, result.Value);
        }


        /// <summary>
        /// Approve a project
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <returns></returns>
        /// <response code="404">Project not found</response>
        /// <response code="204">Success</response>
        [HttpPut("{id}/approve")]
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RejectProject(Guid id)
        {
            var result = await _sender.Send(new RejectProjectCommand(id), HttpContext.RequestAborted);
            if (result.IsFailed)
                return NotFound();
            return NoContent();
        }

    }
}

using Hyme.Application.DTOs.Request;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Queries.Projects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Hyme.API.Controllers.V1
{

    /// <summary>
    /// 
    /// </summary>
    [Route("api/projects")]
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

    }
}

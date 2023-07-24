using FluentResults;
using Hyme.Application.Commands.Users;
using Hyme.Application.DTOs.Request;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Errors;
using Hyme.Application.Queries.UserProfiles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hyme.API.Controllers.V1
{

    /// <summary>
    /// 
    /// </summary>
    [Route("users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ISender _sender;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">MediatR</param>
        public UsersController(ISender sender)
        {
            _sender = sender;
        }


        /// <summary>
        /// List of users
        /// </summary>
        /// <param name="paginationRequest">Page number and page size</param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="400">When page number or papge size cannot be less than 1</response>
        [HttpGet]
        [Authorize(Roles = "Admin, Super Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<UserResponse>>> GetUsers([FromQuery] PaginationRequest paginationRequest)
        {
            if (paginationRequest.PageNumber < 1 || paginationRequest.PageSize < 1)
                return BadRequest("Page number or page size cannot be 0 or negative number");

            PagedResponse<UserResponse> response = await _sender.Send(new GetUsersQuery(paginationRequest.PageNumber, paginationRequest.PageSize));
            return Ok(response);
        }

        /// <summary>
        /// Profile of currently loged-in user
        /// </summary>
        /// <returns></returns>
        /// <response code="404">Profile not found</response>
        /// <response code="401">No user id in the token</response>
        /// <response code="200">Success</response>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> GetMyProfile()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();
            UserResponse? userProfile = await _sender.Send(new GetUserByIdQuery(Guid.Parse(userId)), HttpContext.RequestAborted);
            if (userProfile is null)
                return NotFound();
            return Ok(userProfile);
        }

        /// <summary>
        /// Complete user profile
        /// </summary>
        /// <param name="updateRequest">update data</param>
        /// <returns></returns>
        /// <response code="404">User profile not found</response>
        /// <response code="401">Invalid token</response>
        /// <resopnse code="204">Update successful</resopnse>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateProfile([FromBody]UpdateUserProfileRequest updateRequest)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();

            Result result = await _sender.Send(new UpdateUserCommand(Guid.Parse(userId), updateRequest.Name));
            if (result.IsFailed)
            {
                if (result.HasError(out IEnumerable<ValidationError> errors))
                {
                    foreach (var e in errors)
                    {
                        ModelState.AddModelError(e.PropertyName, e.Message);
                    }
                    return ValidationProblem(ModelState);
                }
                return NotFound();
            }
           
            return NoContent();
        }

    }
}

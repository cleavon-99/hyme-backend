using FluentResults;
using Hyme.Application.Commands.UserProfiles;
using Hyme.Application.DTOs.Request;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Queries.UserProfiles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hyme.API.Controllers.V1
{

    /// <summary>
    /// 
    /// </summary>
    [Route("userProfiles")]
    [ApiController]
    [Authorize]
    public class UserProfilesController : ControllerBase
    {
        private readonly ISender _sender;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">MediatR</param>
        public UserProfilesController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Profile of currently login user
        /// </summary>
        /// <returns></returns>
        /// <response code="404">Profile not found</response>
        /// <response code="401">No user id in the token</response>
        /// <response code="200">Success</response>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfileResponse>> GetMyProfile()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();
            UserProfileResponse? userProfile = await _sender.Send(new GetUserProfleByIdQuery(Guid.Parse(userId)), HttpContext.RequestAborted);
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

            Result result = await _sender.Send(new UpdateUserProfileCommand(Guid.Parse(userId), updateRequest.Name));
            if (result.IsFailed)
                return NotFound();

            return NoContent();
        }

    }
}

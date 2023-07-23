using Hyme.Application.Queries.Whitelists;
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
    [Route("whitelists")]
    [ApiController]
    [Authorize]
    public class WhitelistsController : ControllerBase
    {
        private readonly ISender _sender;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Mediator</param>
        public WhitelistsController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Check if the currently logged-in user is whitelisted
        /// </summary>
        /// <returns></returns>
        /// <response code="404">User is not whitelisted</response>
        /// <response code="200">User is whitelisted</response>
        /// <response code="401">User is not logged-in</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CheckIfWhitelisted()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();
            bool isWhitelisted = await _sender.Send(new CheckUserIfWhitelistedQuery(Guid.Parse(userId)), HttpContext.RequestAborted);
            if (isWhitelisted)
                return Ok();
            return NotFound();
        }
    }
}

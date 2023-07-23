using FluentResults;
using Hyme.Application.Commands.Authentication;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hyme.API.Controllers.V1
{
    /// <summary>
    /// 
    /// </summary>
    [Route("authentication")]
    [ApiController]
    public class AuthenticationController : ApiBase
    {
        private readonly ISender _sender;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">MediatR</param>
        public AuthenticationController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Token
        /// </summary>
        /// <param name="walletRequest">Authentication Data</param>
        /// <returns></returns>
        /// <response code="400">Invalid Signature</response>
        /// <response code="200">Success</response>
        [HttpPost("wallet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthenticationResponse>> ConnectToWallet([FromBody]ConnectToWalletCommand walletRequest)
        {
            Result<AuthenticationResponse> result = await _sender.Send(walletRequest, HttpContext.RequestAborted);
            if (result.IsFailed)
            {
                if (result.HasError(out IEnumerable<ValidationError> errors))
                    return Problem(errors);
                return BadRequest("Invalid signature");
            }      
            return Ok(result.Value);
        }
    }
}

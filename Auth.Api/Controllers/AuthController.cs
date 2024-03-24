using Auth.Application.Command.CreateJwtToken;
using Auth.Application.Command.CreateJwtTokenByRefreshToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers
{
    [Authorize]
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("CreateJwtToken")]
        public async Task<IActionResult> CreateJwtToken(
            CreateJwtTokenCommand command,
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            return Ok(await mediator.Send(command, cancellationToken));
        }

        [AllowAnonymous]
        [HttpPost("CreateJwtTokenByRefreshToken")]
        public async Task<IActionResult> CreateJwtToken(
            CreateJwtTokenByRefreshTokenCommand command,
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            return Ok(await mediator.Send(command, cancellationToken));
        }
    }
}

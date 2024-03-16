using AuthService;
using AuthService.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Auth.Api.Controllers
{
    [Authorize]
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("CreateJwtToken")]
        public async Task<IActionResult> CreateJwtToken(AuthUserDto authDto, CancellationToken cancellationToken)
        {
            return Ok(await _authService.GetJwtTokenAsync(authDto, cancellationToken));
        }

        [AllowAnonymous]
        [HttpPost("CreateJwtTokenByRefreshToken")]
        public async Task<IActionResult> CreateJwtToken(string refreshToken, CancellationToken cancellationToken)
        {
            return Ok(await _authService.GetJwtTokenByRefreshTokenAsync(refreshToken, cancellationToken));
        }
    }
}

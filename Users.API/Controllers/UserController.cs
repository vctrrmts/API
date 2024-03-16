using Microsoft.AspNetCore.Mvc;
using Common.Domain;
using Common.Api.Exceptions;
using Users.Service;
using Users.Service.Dto;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Users.API.Controllers
{
    [Authorize]
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetListAsync(int? offset, int? limit, string? labelFreeText, CancellationToken cancellationToken)
        {
            var users = await _userService.GetListAsync(offset, labelFreeText, limit, cancellationToken);
            int totalCount = await _userService.GetCountAsync(labelFreeText, cancellationToken);
            HttpContext.Response.Headers.Append("X-Total-Count", totalCount.ToString());
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return Ok(await _userService.GetByIdAsync(id, cancellationToken));
        }

        [AllowAnonymous]
        [HttpGet("TotalCount")]
        public async Task<IActionResult> GetCountAsync(string? labelFreeText, CancellationToken cancellationToken)
        {
            return Ok(await _userService.GetCountAsync(labelFreeText, cancellationToken));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserDto user, CancellationToken cancellationToken)
        {
            var newUser = await _userService.CreateAsync(user, cancellationToken);
            return Created("/users/" + newUser.Id, newUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _userService.DeleteAsync(currentUserId, id, cancellationToken);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UpdateUserDto user, CancellationToken cancellationToken)
        {
            var currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Ok(await _userService.UpdateUserAsync(currentUserId ,id, user, cancellationToken));
        }

        [HttpPut("{id}/password")]
        public async Task<IActionResult> UpdatePasswordAsync(int id, string newPassword, CancellationToken cancellationToken)
        {
            var currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _userService.UpdatePasswordAsync(currentUserId, id, newPassword, cancellationToken);
            return Ok();
        }
    }
}

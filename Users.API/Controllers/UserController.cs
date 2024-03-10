using Microsoft.AspNetCore.Mvc;
using Common.Domain;
using Common.Api.Exceptions;
using Users.Service;
using Users.Service.Dto;

namespace Users.API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public IActionResult GetList(int? offset, int? limit, string? labelFreeText)
        {
            var users = _userService.GetList(offset, labelFreeText, limit);
            int totalCount = _userService.GetCount(labelFreeText);
            HttpContext.Response.Headers.Append("X-Total-Count", totalCount.ToString());
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);
            return Ok(user);
        }

        [HttpGet("TotalCount")]
        public IActionResult GetCount(string? labelFreeText)
        {
            return Ok(_userService.GetCount(labelFreeText));
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateUserDto user)
        {
            var newUser = _userService.Create(user);
            return Created("/users/" + newUser.Id, newUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await _userService.DeleteAsync(id, cancellationToken);
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateUserDto user)
        {
            var updateResult = _userService.Update(id, user);
            return Ok(updateResult);
        }
    }
}

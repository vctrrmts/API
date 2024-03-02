using Microsoft.AspNetCore.Mvc;
using Common.Domain;
using Common.Application;
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
        public IActionResult GetById(int id)
        {
            try
            {
                var user = _userService.GetById(id);
                return Ok(user);
            }
            catch (NotFoundException)
            {
                return NotFound(id);
            }
        }

        [HttpGet("TotalCount")]
        public IActionResult GetCount(string? labelFreeText)
        {
            return Ok(_userService.GetCount(labelFreeText));
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateUserDto user)
        {
            try
            {
                User newUser = _userService.Create(user);

                return Created("/users/" + newUser.Id, newUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _userService.Delete(id);
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateUserDto user)
        {
            try
            {
                var updateResult = _userService.Update(id, user);
                return Ok(updateResult);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }         
        }
    }
}

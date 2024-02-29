using Microsoft.AspNetCore.Mvc;
using Common.Domain;
using Users.Service;

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
        public IActionResult GetList(int? offset, int limit = 5)
        {
            var users = _userService.GetList(offset, limit);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);

            if (user == null) return NotFound(id);
            return Ok(user);
        }

        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            User newUser = _userService.Post(user);

            return Created("/users/" + newUser.Id, newUser);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_userService.Delete(id)) return NotFound();
            return Ok();

        }

        [HttpPatch("{id}/Name")]
        public IActionResult Patch(int id, [FromBody] string name)
        {
            try
            {
                var changeNameResult = _userService.Patch(id, name);
                return Ok(changeNameResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

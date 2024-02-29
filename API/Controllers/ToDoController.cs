using Microsoft.AspNetCore.Mvc;
using Todos.Domain;
using Todos.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todos.API.Controllers
{
    [Route("todos")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly IToDoService _toDoService;

        public ToDoController(IToDoService toDoService) 
        {
            _toDoService = toDoService;
        }


        [HttpGet]
        public IActionResult GetList(int? offset, int? ownerId, string? labelFreeText, int limit = 10)
        {
            var todos = _toDoService.GetList(offset, ownerId, labelFreeText, limit);
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var todo = _toDoService.GetById(id);

            if (todo == null) return NotFound(id);
            return Ok(todo);
        }

        [HttpGet("{id}/IsDone")]
        public IActionResult GetIsDone(int id)
        {
            var isdone = _toDoService.GetIsDone(id);

            if (isdone == null) return NotFound(id);
            return Ok(isdone);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ToDo todo)
        {
            ToDo? newtodo = _toDoService.Post(todo);
            if (newtodo == null)
            {
                return BadRequest("OwnerId does not exist");
            }
            else
            {
                return Created("/todos/" + newtodo.Id, newtodo);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ToDo todo)
        {
            try
            {
                var updatedTodo = _toDoService.Put(id, todo);
                if (updatedTodo == null) return NotFound(id);
                return Ok(updatedTodo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPatch("{id}/IsDone")]
        public IActionResult Patch(int id, [FromBody] bool isDone)
        {
            var isDoneResult = _toDoService.Patch(id, isDone);
            if (isDoneResult == null) return NotFound(id);
            return Ok(isDoneResult);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_toDoService.Delete(id)) return NotFound();
            return Ok();
            
        }
    }
}

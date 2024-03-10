using Microsoft.AspNetCore.Mvc;
using Common.Domain;
using Todos.Service;
using Todos.Service.Dto;
using Todos.Service.Models;

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
            int totalCount = _toDoService.GetCount(labelFreeText);
            HttpContext.Response.Headers.Append("X-Total-Count", totalCount.ToString());
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var todo = await _toDoService.GetByIdAsync(id, cancellationToken);
            return Ok(todo);
        }

        [HttpGet("TotalCount")]
        public IActionResult GetCount(string? labelFreeText)
        {
            return Ok(_toDoService.GetCount(labelFreeText));
        }

        [HttpGet("{id}/IsDone")]
        public async Task<IActionResult> GetIsDoneAsync(int id, CancellationToken cancellationToken)
        {
            var isdone = await _toDoService.GetIsDoneAsync(id, cancellationToken);
            return Ok(isdone);
        }

        [HttpPost]
        public IActionResult Post([FromBody] CreateToDoDto todo)
        {
            var newtodo = _toDoService.Create(todo);
            return Created("/todos/" + newtodo.Id, newtodo);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateToDoDto todo)
        {
            var updatedTodo = _toDoService.Update(id, todo);
            return Ok(updatedTodo);

        }

        [HttpPatch("{id}/IsDone")]
        public IActionResult Patch(int id, [FromBody] bool isDone)
        {
            var isDoneResult = _toDoService.Patch(id, isDone);
            return Ok(isDoneResult);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await _toDoService.DeleteAsync(id, cancellationToken);
            return Ok();
        }
    }
}

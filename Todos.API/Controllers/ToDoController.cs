using Microsoft.AspNetCore.Mvc;
using Common.Domain;
using Todos.Service;
using Todos.Service.Dto;
using Todos.Service.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Todos.API.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> GetListAsync(int? offset, int? ownerId, string? labelFreeText, 
            int limit = 10, CancellationToken cancellationToken = default)
        {
            var currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var todos = await _toDoService.GetListAsync(currentUserId, offset, ownerId, labelFreeText, limit,  cancellationToken);
            int totalCount = await _toDoService.GetCountAsync(currentUserId, ownerId, labelFreeText, cancellationToken);
            HttpContext.Response.Headers.Append("X-Total-Count", totalCount.ToString());
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var todo = await _toDoService.GetByIdAsync(currentUserId, id, cancellationToken);
            return Ok(todo);
        }

        [HttpGet("{id}/IsDone")]
        public async Task<IActionResult> GetIsDoneAsync(int id, CancellationToken cancellationToken)
        {
            var currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var isdone = await _toDoService.GetIsDoneAsync(currentUserId, id, cancellationToken);
            return Ok(isdone);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] CreateToDoDto todo, CancellationToken cancellationToken)
        {
            var currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var newtodo = await _toDoService.CreateAsync(currentUserId, todo, cancellationToken);
            return Created("/todos/" + newtodo.Id, newtodo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateToDoDto todo, CancellationToken cancellationToken)
        {
            var currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var updatedTodo = await _toDoService.UpdateAsync(currentUserId, id, todo, cancellationToken);
            return Ok(updatedTodo);
        }

        [HttpPatch("{id}/IsDone")]
        public async Task<IActionResult> PatchAsync(int id, [FromBody] bool isDone, CancellationToken cancellationToken)
        {
            var currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var isDoneResult = await _toDoService.PatchAsync(currentUserId, id, isDone, cancellationToken);
            return Ok(isDoneResult);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _toDoService.DeleteAsync(currentUserId, id, cancellationToken);
            return Ok();
        }
    }
}

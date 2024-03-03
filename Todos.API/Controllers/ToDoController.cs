using Microsoft.AspNetCore.Mvc;
using Todos.Domain;
using Todos.Service;
using Todos.Service.Dto;
using Todos.Service.Models;
using Common.Application;

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
        public IActionResult GetById(int id)
        {
            try
            {
                var todo = _toDoService.GetById(id);
                return Ok(todo);
            }
            catch (NotFoundException)
            {
                return NotFound(id);
            }
        }

        [HttpGet("TotalCount")]
        public IActionResult GetCount(string? labelFreeText)
        {
            return Ok(_toDoService.GetCount(labelFreeText));
        }

        [HttpGet("{id}/IsDone")]
        public IActionResult GetIsDone(int id)
        {
            try
            {
                var isdone = _toDoService.GetIsDone(id);
                return Ok(isdone);
            }
            catch (NotFoundException)
            {
                return NotFound(id);
            } 
        }

        [HttpPost]
        public IActionResult Post([FromBody] CreateToDoDto todo)
        {
            try
            {
                ToDo newtodo = _toDoService.Create(todo);
                return Created("/todos/" + newtodo.Id, newtodo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateToDoDto todo)
        {
            try
            {
                var updatedTodo = _toDoService.Update(id, todo);
                return Ok(updatedTodo);
            }
            catch (NotFoundException)
            {
                return NotFound(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPatch("{id}/IsDone")]
        public IActionResult Patch(int id, [FromBody] bool isDone)
        {
            try
            {
                var isDoneResult = _toDoService.Patch(id, isDone);
                return Ok(isDoneResult);
            }
            catch (NotFoundException)
            {
                return NotFound(id);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _toDoService.Delete(id);
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }          
        }
    }
}

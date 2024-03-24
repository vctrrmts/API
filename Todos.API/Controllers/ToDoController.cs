using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Todos.Application.Query.GetList;
using MediatR;
using Todos.Application.Query.GetCount;
using Todos.Application.Query.GetById;
using Todos.Application.Command.CreateTodo;
using Todos.Application.Command.UpdateTodo;
using Todos.Application.Command.PatchIsDone;
using Todos.Application.Command.DeleteTodo;
using Todos.Application.Query.GetIsDone;
using Todos.Application.Dtos;

namespace Todos.API.Controllers
{
    [Authorize]
    [Route("todos")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetListAsync(
            [FromQuery] GetListQuery getListQuery,
            IMediator mediator,
            CancellationToken cancellationToken = default)
        {
            var todos = await mediator.Send(getListQuery, cancellationToken);
            int totalCount = await mediator.Send(
                new GetCountQuery() { LabelFreeText = getListQuery.LabelFreeText, OwnerId = getListQuery.OwnerId },
                cancellationToken);
            HttpContext.Response.Headers.Append("X-Total-Count", totalCount.ToString());
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(
            int id, 
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            var todo = await mediator.Send(new GetByIdQuery() {Id = id }, cancellationToken);
            return Ok(todo);
        }

        [HttpGet("{id}/IsDone")]
        public async Task<IActionResult> GetIsDoneAsync(
            int id, 
            IMediator mediator, 
            CancellationToken cancellationToken)
        {
            var isdone = await mediator.Send(new GetIsDoneQuery() {Id = id}, cancellationToken);
            return Ok(isdone);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            CreateTodoCommand createTodoCommand,
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            var newtodo = await mediator.Send(createTodoCommand, cancellationToken);
            return Created("/todos/" + newtodo.Id, newtodo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            int id,
            UpdateTodoCommandDto dto,
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            var updatedTodo = await mediator.Send(
                new UpdateTodoCommand() {Id = id, IsDone = dto.IsDone, Label = dto.Label }, 
                cancellationToken);
            return Ok(updatedTodo);
        }

        [HttpPatch("{id}/IsDone")]
        public async Task<IActionResult> PatchAsync(
            int id,
            PatchIsDoneCommandDto dto,
            IMediator mediator, 
            CancellationToken cancellationToken)
        {
            var isDoneResult = await mediator.Send(new PatchIsDoneCommand() {Id = id, IsDone = dto.IsDone }, cancellationToken);
            return Ok(isDoneResult);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(
            int id,
            IMediator mediator, 
            CancellationToken cancellationToken)
        {
            await mediator.Send(new DeleteTodoCommand() {Id = id}, cancellationToken);
            return Ok();
        }
    }
}

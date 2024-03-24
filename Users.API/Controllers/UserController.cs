using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Users.Application.Command.CreateUser;
using MediatR;
using Users.Application.Query.GetList;
using Users.Application.Query.GetCount;
using Users.Application.Query.GetById;
using Users.Application.Command.DeleteUser;
using Users.Application.Command.UpdateUser;
using Users.Application.Command.UpdatePassword;
using Users.Application.Dtos;

namespace Users.API.Controllers
{
    [Authorize]
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetListAsync(
            [FromQuery] GetListQuery getListQuery, 
            IMediator mediator, 
            CancellationToken cancellationToken)
        {
            var users = await mediator.Send(getListQuery, cancellationToken);
            int totalCount = await mediator.Send(new GetCountQuery() {NameFreeText = getListQuery.NameFreeText }, cancellationToken);
            HttpContext.Response.Headers.Append("X-Total-Count", totalCount.ToString());
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(
            int id, 
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            return Ok(await mediator.Send( new GetByIdQuery() {Id = id }, cancellationToken));
        }

        [AllowAnonymous]
        [HttpGet("TotalCount")]
        public async Task<IActionResult> GetCountAsync(
            [FromQuery] GetCountQuery getCountQuery, 
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            return Ok(await mediator.Send(getCountQuery, cancellationToken));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            CreateUserCommand createUserCommand,
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            var newUser = await mediator.Send(createUserCommand, cancellationToken);
            return Created("/users/" + newUser.Id, newUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(
            int id,
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            await mediator.Send(new DeleteUserCommand() {Id = id }, cancellationToken);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(
            int id, 
            UpdateUserCommandDto dto,
            IMediator mediator, 
            CancellationToken cancellationToken)
        {
            UpdateUserCommand updateUserCommand = new UpdateUserCommand() {Id = id, Login = dto.Login };
            return Ok(await mediator.Send(updateUserCommand, cancellationToken));
        }

        [HttpPut("{id}/password")]
        public async Task<IActionResult> UpdatePasswordAsync(
            int id,
            UpdatePasswordCommandDto dto,
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            await mediator.Send(new UpdatePasswordCommand() {Id = id, Password = dto.Password }, cancellationToken);
            return Ok();
        }
    }
}

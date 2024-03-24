using MediatR;
using Todos.Application.Dtos;

namespace Todos.Application.Command.CreateTodo
{
    public class CreateTodoCommand : IRequest<GetTodoDto>
    {
        public string Label { get; set; } = default!;
        public bool IsDone { get; set; }
    }
}

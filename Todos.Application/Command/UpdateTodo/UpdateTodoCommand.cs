using MediatR;
using Todos.Application.Dtos;

namespace Todos.Application.Command.UpdateTodo
{
    public class UpdateTodoCommand : IRequest<GetTodoDto>
    {
        public int Id { get; set; }
        public string Label { get; set; } = default!;
        public bool IsDone { get; set; }
    }
}

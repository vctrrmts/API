using MediatR;

namespace Todos.Application.Command.DeleteTodo
{
    public class DeleteTodoCommand : IRequest
    {
        public int Id { get; set; }
    }
}

using MediatR;
using Todos.Application.Dtos;

namespace Todos.Application.Query.GetById
{
    public class GetByIdQuery : IRequest<GetTodoDto>
    {
        public int Id { get; set; }
    }
}

using MediatR;
using Todos.Application.Dtos;

namespace Todos.Application.Query.GetList
{
    public class GetListQuery : BaseTodosFilter, IRequest<IReadOnlyCollection<GetTodoDto>>
    {
        public int? Offset { get; set; }
        public int? Limit { get; set; }
    }
}

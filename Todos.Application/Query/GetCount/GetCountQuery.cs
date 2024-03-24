using MediatR;
using Todos.Application.Dtos;

namespace Todos.Application.Query.GetCount
{
    public class GetCountQuery : BaseTodosFilter, IRequest<int>
    {
    }
}

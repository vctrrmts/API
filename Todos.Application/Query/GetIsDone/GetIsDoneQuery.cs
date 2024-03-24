using MediatR;
using Todos.Application.Models;

namespace Todos.Application.Query.GetIsDone
{
    public class GetIsDoneQuery : IRequest<IsDoneResult>
    {
        public int Id { get; set; }
    }
}

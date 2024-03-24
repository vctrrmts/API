using MediatR;
using Todos.Application.Models;

namespace Todos.Application.Command.PatchIsDone
{
    public class PatchIsDoneCommand : IRequest<IsDoneResult>
    {
        public int Id { get; set; }
        public bool IsDone { get; set; }
    }
}

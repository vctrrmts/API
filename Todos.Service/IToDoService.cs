using Todos.Domain;
using Todos.Service.Dto;

namespace Todos.Service
{
    public interface IToDoService
    {
        IReadOnlyCollection<ToDo> GetList(int? offset, int? ownerId, string? labelFreeText, int? limit);
        ToDo? GetById(int id);
        IsDoneResult? GetIsDone(int id);
        ToDo Create(CreateToDoDto todo);
        ToDo? Update(int id, UpdateToDoDto todo);
        IsDoneResult? Patch(int id, bool isDone);
        bool Delete(int id);
        int GetCount(string? labelFreeText);
    }
}

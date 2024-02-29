using Todos.Domain;

namespace Todos.Service
{
    public interface IToDoService
    {
        IReadOnlyCollection<ToDo> GetList(int? offset, int? ownerId, string? labelFreeText, int limit);
        ToDo? GetById(int id);
        IsDoneResult? GetIsDone(int id);
        ToDo? Post(ToDo todo);
        ToDo? Put(int id, ToDo todo);
        IsDoneResult? Patch(int id, bool isDone);
        bool Delete(int id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Domain;

namespace Todos.Repositories
{
    public interface IToDoRepository
    {
        IReadOnlyCollection<ToDo> GetList(int? offset, int? ownerId, string? labelFreeText, int limit);
        ToDo? GetById(int id);
        ToDo Post(ToDo todo);
        ToDo? Put(ToDo todoForUpdate, ToDo todo);
        IsDoneResult? Patch(ToDo todo, bool isDone);
        bool Delete(ToDo todo);
    }
}

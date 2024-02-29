using Todos.Domain;

namespace Todos.Repositories
{
    public class ToDoRepository : IToDoRepository
    {
        private static List<ToDo> ToDoList = new List<ToDo>()
        { new ToDo{Id = 1, Label = "todo1", OwnerId = 1, CreatedTime = DateTime.UtcNow},
          new ToDo{Id = 2, Label = "todo2", OwnerId = 2, CreatedTime = DateTime.UtcNow},
          new ToDo{Id = 3, Label = "todo3", OwnerId = 3, CreatedTime = DateTime.UtcNow},
          new ToDo{Id = 4, Label = "todo4", OwnerId = 1, CreatedTime = DateTime.UtcNow},
          new ToDo{Id = 5, Label = "todo5", OwnerId = 2, CreatedTime = DateTime.UtcNow, IsDone = true},
          new ToDo{Id = 6, Label = "todo6", OwnerId = 3, CreatedTime = DateTime.UtcNow, IsDone = true},
          new ToDo{Id = 7, Label = "todo7", OwnerId = 1, CreatedTime = DateTime.UtcNow, IsDone = true},
          new ToDo{Id = 8, Label = "todo8", OwnerId = 2, CreatedTime = DateTime.UtcNow, IsDone = true}};

        public IReadOnlyCollection<ToDo> GetList(int? offset, int? ownerId, string? labelFreeText, int limit)
        {
            IEnumerable<ToDo> todos = ToDoList.OrderBy(x=>x.Id);
            if (ownerId != null)
            {
                todos = todos.Where(x => x.OwnerId == ownerId);
            }

            if (!string.IsNullOrWhiteSpace(labelFreeText)) 
            {
                todos = todos.Where(x => x.Label.Contains(labelFreeText, StringComparison.InvariantCultureIgnoreCase));
            }

            if (offset.HasValue) 
            {
                todos = todos.Skip(offset.Value);
            }

            return todos.Take(limit).ToList();
        }

        public ToDo? GetById(int id)
        {
            ToDo result = ToDoList.SingleOrDefault(x => x.Id == id)!;
            return result;
        }

        public ToDo Post(ToDo newTodo)
        {
            newTodo.Id = ToDoList.Max(x => x.Id) + 1;
            ToDoList.Add(newTodo);
            return newTodo;
        }

        public ToDo? Put(ToDo todoForUpdate, ToDo todo)
        {
            todoForUpdate.Label = todo.Label;
            todoForUpdate.OwnerId = todo.OwnerId;
            todoForUpdate.IsDone = todo.IsDone;
            todoForUpdate.UpdatedTime = DateTime.UtcNow;
            return todoForUpdate;
        }

        public IsDoneResult? Patch(ToDo todo, bool isDone)
        {
            todo.IsDone = isDone;
            return new IsDoneResult() { Id = todo.Id, IsDone = todo.IsDone };
        }

        public bool Delete(ToDo todo)
        {
            ToDoList.Remove(todo);
            return true;
        }
    }
}

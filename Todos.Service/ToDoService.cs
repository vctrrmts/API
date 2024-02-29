using Todos.Domain;
using Todos.Repositories;
using Common.Repositories;

namespace Todos.Service
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IUserRepository _userRepository;


        public ToDoService(IToDoRepository toDoRepository, IUserRepository userRepository)
        {
            _toDoRepository = toDoRepository;
            _userRepository = userRepository;
        }

        public IReadOnlyCollection<ToDo> GetList(int? offset, int? ownerId, string? labelFreeText, int limit)
        {
            return _toDoRepository.GetList(offset, ownerId, labelFreeText, limit);
        }

        public ToDo? GetById(int id)
        {
            return _toDoRepository.GetById(id);
        }

        public IsDoneResult? GetIsDone(int id)
        {
            ToDo? todo = _toDoRepository.GetById(id);
            if (todo == null) return null;

            return new IsDoneResult() { Id = id, IsDone = todo.IsDone };
        }

        public ToDo? Post( ToDo todo)
        {
            if (_userRepository.GetById(todo.OwnerId) == null)
            {
                return null;
            }

            ToDo newTodo = new ToDo
            {
                Label = todo.Label,
                OwnerId = todo.OwnerId,
                IsDone = todo.IsDone,
                CreatedTime = DateTime.UtcNow
            };

            return _toDoRepository.Post(newTodo);
        }

        public ToDo? Put(int id, ToDo todo)
        {
            ToDo? todoForUpdate = _toDoRepository.GetById(id);
            if (todoForUpdate == null) return null;

            var user = _userRepository.GetById(todo.OwnerId);
            if (user == null)
                throw new Exception($"OwnerId does not exist");

            return _toDoRepository.Put(todoForUpdate, todo);
        }

        public IsDoneResult? Patch(int id, bool isDone)
        {
            ToDo? todo = _toDoRepository.GetById(id);
            if (todo == null) return null;

            return _toDoRepository.Patch(todo, isDone);
        }

        public bool Delete(int id)
        {
            ToDo? todoById = _toDoRepository.GetById(id);
            if (todoById == null) return false;

            return _toDoRepository.Delete(todoById);
        }
    }
}

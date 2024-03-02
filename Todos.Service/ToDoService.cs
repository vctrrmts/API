using Todos.Domain;
using Common.Repositories;
using Common.Domain;
using Common.Application;
using System.Linq.Expressions;
using Todos.Service.Dto;
using Todos.Service.Models;
using AutoMapper;

namespace Todos.Service
{
    public class ToDoService : IToDoService
    {
        private readonly IRepository<ToDo> _toDoRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;


        public ToDoService(IRepository<ToDo> toDoRepository, IRepository<User> userRepository, IMapper mapper)
        {
            _toDoRepository = toDoRepository;
            _userRepository = userRepository;
            _mapper = mapper;

            if (_toDoRepository.GetList().Length == 0)
            {
                _toDoRepository.Add(new ToDo { Id = 1, Label = "todo1", OwnerId = 1, CreatedTime = DateTime.UtcNow });
                _toDoRepository.Add(new ToDo { Id = 2, Label = "todo2", OwnerId = 2, CreatedTime = DateTime.UtcNow });
                _toDoRepository.Add(new ToDo { Id = 3, Label = "todo3", OwnerId = 3, CreatedTime = DateTime.UtcNow });
                _toDoRepository.Add(new ToDo { Id = 4, Label = "todo4", OwnerId = 1, CreatedTime = DateTime.UtcNow });
                _toDoRepository.Add(new ToDo { Id = 5, Label = "todo5", OwnerId = 2, CreatedTime = DateTime.UtcNow, IsDone = true });
                _toDoRepository.Add(new ToDo { Id = 6, Label = "todo6", OwnerId = 3, CreatedTime = DateTime.UtcNow, IsDone = true });
                _toDoRepository.Add(new ToDo { Id = 7, Label = "todo7", OwnerId = 1, CreatedTime = DateTime.UtcNow, IsDone = true });
                _toDoRepository.Add(new ToDo { Id = 8, Label = "todo8", OwnerId = 2, CreatedTime = DateTime.UtcNow, IsDone = true });
            }

            if (_userRepository.GetList().Length == 0)
            {
                _userRepository.Add(new User() { Id = 1, Name = "Viktor" });
                _userRepository.Add(new User() { Id = 2, Name = "Igor" });
                _userRepository.Add(new User() { Id = 3, Name = "Gennadiy" });
            }

        }

        public IReadOnlyCollection<ToDo> GetList(int? offset, int? ownerId, string? labelFreeText, int? limit = 10)
        {
            Expression<Func<ToDo, bool>>? expression = null;
            if (ownerId != null)
            {
                if (!string.IsNullOrWhiteSpace(labelFreeText))
                    expression = x => x.OwnerId == ownerId && x.Label.Contains(labelFreeText, StringComparison.InvariantCultureIgnoreCase);
                else expression = x => x.OwnerId == ownerId;
            }
            else if(!string.IsNullOrWhiteSpace(labelFreeText)) 
                expression = x=>x.Label.Contains(labelFreeText, StringComparison.InvariantCultureIgnoreCase);

            return _toDoRepository.GetList(offset, limit, expression, x=>x.Id);
        }

        public ToDo? GetById(int id)
        {
            ToDo? todo = _toDoRepository.SingleOrDefault(x=>x.Id == id);
            if (todo == null) throw new NotFoundException();
            return todo;
        }

        public IsDoneResult? GetIsDone(int id)
        {
            ToDo? todo = _toDoRepository.SingleOrDefault(x=>x.Id == id);
            if (todo == null) throw new NotFoundException();

            return new IsDoneResult() { Id = id, IsDone = todo.IsDone };
        }

        public ToDo Create( CreateToDoDto todoDto)
        {
            if (_userRepository.SingleOrDefault(x=>x.Id == todoDto.OwnerId) == null)
                throw new Exception("Owner id does not exist");

            if (string.IsNullOrWhiteSpace(todoDto.Label))
                throw new Exception("Label must not be empty");

            int idNew = _toDoRepository.GetList().Length == 0 ? 1 : _toDoRepository.GetList().Max(x=>x.Id) + 1;

            ToDo newTodo = _mapper.Map<CreateToDoDto, ToDo>(todoDto);
            newTodo.Id = idNew;
            newTodo.CreatedTime = DateTime.UtcNow;
           
            return _toDoRepository.Add(newTodo);
        }

        public ToDo? Update(int id, UpdateToDoDto newTodo)
        {
            if (_userRepository.SingleOrDefault(x => x.Id == newTodo.OwnerId) == null)
                throw new Exception("Owner id does not exist");

            if (string.IsNullOrWhiteSpace(newTodo.Label))
                throw new Exception("Label must not be empty");

            ToDo? todoForUpdate = _toDoRepository.SingleOrDefault(x=>x.Id == id);
            if (todoForUpdate == null) throw new NotFoundException();

            _mapper.Map<UpdateToDoDto, ToDo>(newTodo, todoForUpdate);
            todoForUpdate.UpdatedTime = DateTime.UtcNow;

            return _toDoRepository.Update(todoForUpdate);
        }

        public IsDoneResult? Patch(int id, bool isDone)
        {
            ToDo? todo = _toDoRepository.SingleOrDefault(x=>x.Id == id);
            if (todo == null) throw new NotFoundException();

            todo.IsDone = isDone;
            _toDoRepository.Update(todo);
            return new IsDoneResult() {Id = todo.Id, IsDone = todo.IsDone };
        }

        public bool Delete(int id)
        {
            ToDo? todoById = _toDoRepository.SingleOrDefault(x=>x.Id == id);
            if (todoById == null) throw new NotFoundException();

            return _toDoRepository.Delete(todoById);
        }

        public int GetCount(string? labelFreeText)
        {
             return _toDoRepository.Count(labelFreeText == null ? null : x => x.Label.Contains(labelFreeText, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}

using Common.Domain;
using Common.Repositories;
using Common.Api.Exceptions;
using System.Linq.Expressions;
using Todos.Service.Dto;
using Todos.Service.Models;
using AutoMapper;
using Serilog;
using System.Text.Json;

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

            if (_userRepository.GetList().Length == 0)
            {
                _userRepository.Add(new User() { Name = "Viktor" });
                _userRepository.Add(new User() { Name = "Igor" });
                _userRepository.Add(new User() { Name = "Gennadiy" });
            }

            if (_toDoRepository.GetList().Length == 0)
            {
                _toDoRepository.Add(new ToDo { Label = "todo1", OwnerId = 1, CreatedTime = DateTime.UtcNow });
                _toDoRepository.Add(new ToDo { Label = "todo2", OwnerId = 2, CreatedTime = DateTime.UtcNow });
                _toDoRepository.Add(new ToDo { Label = "todo3", OwnerId = 3, CreatedTime = DateTime.UtcNow });
                _toDoRepository.Add(new ToDo { Label = "todo4", OwnerId = 1, CreatedTime = DateTime.UtcNow });
                _toDoRepository.Add(new ToDo { Label = "todo5", OwnerId = 2, CreatedTime = DateTime.UtcNow, IsDone = true });
                _toDoRepository.Add(new ToDo { Label = "todo6", OwnerId = 3, CreatedTime = DateTime.UtcNow, IsDone = true });
                _toDoRepository.Add(new ToDo { Label = "todo7", OwnerId = 1, CreatedTime = DateTime.UtcNow, IsDone = true });
                _toDoRepository.Add(new ToDo { Label = "todo8", OwnerId = 2, CreatedTime = DateTime.UtcNow, IsDone = true });
            }

        }

        public IReadOnlyCollection<MainToDoDto> GetList(int? offset, int? ownerId, string? labelFreeText, int? limit = 10)
        {
            Expression<Func<ToDo, bool>>? expression = null;
            if (ownerId != null)
            {
                if (!string.IsNullOrWhiteSpace(labelFreeText))
                    expression = x => x.OwnerId == ownerId && x.Label.Contains(labelFreeText);
                else expression = x => x.OwnerId == ownerId;
            }
            else if(!string.IsNullOrWhiteSpace(labelFreeText)) 
                expression = x=>x.Label.Contains(labelFreeText);

            ToDo[] todoList = _toDoRepository.GetList(offset, limit, expression, x => x.Id);

            MainToDoDto[] mainTodoList = new MainToDoDto[todoList.Length];
            for (int i = 0; i < todoList.Length; i++)
            {
                mainTodoList[i] = _mapper.Map<ToDo, MainToDoDto>(todoList[i]);
            }
            return mainTodoList;
        }

        public ToDo GetById(int id)
        {
            ToDo? todo = _toDoRepository.SingleOrDefault(x=>x.Id == id);
            if (todo == null)
            {
                Log.Error("ToDo not found");
                throw new NotFoundException($"ToDo with id = {id} not found");
            }

            return todo;
        }

        public async Task<MainToDoDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            ToDo? todo = await _toDoRepository.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (todo == null)
            {
                Log.Error($"ToDo with id = {id} not found");
                throw new NotFoundException($"ToDo with id = {id} not found");
            }

            return _mapper.Map<ToDo, MainToDoDto>(todo);
        }

        public async Task<IsDoneResult> GetIsDoneAsync(int id, CancellationToken cancellationToken)
        {
            ToDo? todo = await _toDoRepository.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (todo == null)
            {
                Log.Error($"ToDo with id = {id} not found");
                throw new NotFoundException($"ToDo with id = {id} not found");
            }

            return new IsDoneResult() { Id = id, IsDone = todo.IsDone };
        }

        public MainToDoDto Create(CreateToDoDto todoDto)
        {
            if (_userRepository.SingleOrDefault(x => x.Id == todoDto.OwnerId) == null)
            {
                Log.Error($"Owner id {todoDto.OwnerId} does not exist");
                throw new BadRequestException($"Owner id {todoDto.OwnerId} does not exist");
            }

            if (string.IsNullOrWhiteSpace(todoDto.Label))
            {
                Log.Error("Label must not be empty");
                throw new BadRequestException("Label must not be empty");
            }

            ToDo newTodo = _mapper.Map<CreateToDoDto, ToDo>(todoDto);
            newTodo.CreatedTime = DateTime.UtcNow;

            _toDoRepository.Add(newTodo);

            MainToDoDto newMainToDo = _mapper.Map<ToDo, MainToDoDto>(newTodo);
            Log.Information("ToDo added " + JsonSerializer.Serialize(newMainToDo));
            return newMainToDo;
        }

        public MainToDoDto Update(int id, UpdateToDoDto newTodo)
        {
            if (_userRepository.SingleOrDefault(x => x.Id == newTodo.OwnerId) == null)
            {
                Log.Error($"Owner id {newTodo.OwnerId} does not exist");
                throw new BadRequestException($"Owner id {newTodo.OwnerId} does not exist");
            }

            if (string.IsNullOrWhiteSpace(newTodo.Label))
            {
                Log.Error("Label must not be empty");
                throw new BadRequestException("Label must not be empty");
            }

            ToDo? todoForUpdate = _toDoRepository.SingleOrDefault(x=>x.Id == id);
            if (todoForUpdate == null)
            {
                Log.Error($"ToDo with id = {id} not found");
                throw new NotFoundException($"ToDo with id = {id} not found");
            }

            _mapper.Map<UpdateToDoDto, ToDo>(newTodo, todoForUpdate);
            todoForUpdate.UpdatedTime = DateTime.UtcNow;

            _toDoRepository.Update(todoForUpdate);

            MainToDoDto updatedMainToDo = _mapper.Map<ToDo, MainToDoDto>(todoForUpdate);
            Log.Information("ToDo updated " + JsonSerializer.Serialize(updatedMainToDo));
            return updatedMainToDo;
        }

        public IsDoneResult Patch(int id, bool isDone)
        {
            ToDo? todo = _toDoRepository.SingleOrDefault(x=>x.Id == id);
            if (todo == null)
            {
                Log.Error($"ToDo with id = {id} not found");
                throw new NotFoundException($"ToDo with id = {id} not found");
            }

            todo.IsDone = isDone;
            _toDoRepository.Update(todo);

            Log.Information("ToDo patched " + JsonSerializer.Serialize(_mapper.Map<ToDo,MainToDoDto>(todo)));
            return new IsDoneResult() {Id = todo.Id, IsDone = todo.IsDone };
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            ToDo? todoById = await _toDoRepository.SingleOrDefaultAsync(x=>x.Id == id, cancellationToken);
            if (todoById == null)
            {
                Log.Error($"ToDo with id = {id} not found");
                throw new NotFoundException($"ToDo with id = {id} not found");
            }

            _toDoRepository.Delete(todoById);
            Log.Information("ToDo deleted " + JsonSerializer.Serialize(_mapper.Map<ToDo, MainToDoDto>(todoById)));
            return true;
        }

        public int GetCount(string? labelFreeText)
        {
             return _toDoRepository.Count(labelFreeText == null ? null : x => x.Label.Contains(labelFreeText));
        }
    }
}

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
        private readonly IRepository<ApplicationUserApplicationRole> _rolesRepository;
        private readonly IMapper _mapper;

        public ToDoService(
            IRepository<ToDo> toDoRepository, 
            IRepository<ApplicationUserApplicationRole> userRoleRepository,
            IMapper mapper)
        {
            _toDoRepository = toDoRepository;
            _rolesRepository = userRoleRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<GetToDoDto>> GetListAsync(int currentUserId, int? offset, int? ownerId, 
            string? labelFreeText, int? limit = 10, CancellationToken cancellationToken = default)
        {
            var currentUserRoles = await _rolesRepository.GetListAsync(
                expression: x => x.ApplicationUserId == currentUserId,
                cancellationToken: cancellationToken);

            bool isCurrentUserAdmin = currentUserRoles.Any(t => t.ApplicationUserRole.Name == "Admin");

            var todoList = await _toDoRepository.GetListAsync(
                offset, 
                limit,
                t => (string.IsNullOrWhiteSpace(labelFreeText) || t.Label.Contains(labelFreeText)) 
                && (ownerId == null || t.OwnerId == ownerId)
                && (isCurrentUserAdmin || t.OwnerId == currentUserId),
                x => x.Id, 
                false, 
                cancellationToken);
            return _mapper.Map<IReadOnlyCollection<GetToDoDto>>(todoList);
        }

        public async Task<GetToDoDto> GetByIdAsync(int currentUserId, int id, CancellationToken cancellationToken = default)
        {
            ToDo? todo = await _toDoRepository.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (todo == null)
            {
                Log.Error($"ToDo with id = {id} not found");
                throw new NotFoundException($"ToDo with id = {id} not found");
            }

            var currentUserRoles = await _rolesRepository.GetListAsync(
                expression: x => x.ApplicationUserId == currentUserId,
                cancellationToken: cancellationToken);
            if (!currentUserRoles.Any(t => t.ApplicationUserRole.Name == "Admin") && currentUserId != todo.OwnerId)
            {
                throw new ForbiddenException();
            }

            return _mapper.Map<ToDo, GetToDoDto>(todo);
        }

        public async Task<IsDoneResult> GetIsDoneAsync(int currentUserId, int id, CancellationToken cancellationToken = default)
        {
            ToDo? todo = await _toDoRepository.SingleOrDefaultAsync(x => x.Id == id, cancellationToken = default);
            if (todo == null)
            {
                Log.Error($"ToDo with id = {id} not found");
                throw new NotFoundException($"ToDo with id = {id} not found");
            }

            var currentUserRoles = await _rolesRepository.GetListAsync(
                expression: x => x.ApplicationUserId == currentUserId,
                cancellationToken: cancellationToken);
            if (!currentUserRoles.Any(t => t.ApplicationUserRole.Name == "Admin") && currentUserId != todo.OwnerId)
            {
                throw new ForbiddenException();
            }

            return new IsDoneResult() { Id = id, IsDone = todo.IsDone };
        }

        public async Task<GetToDoDto> CreateAsync(int currentUserId,CreateToDoDto todoDto, CancellationToken cancellationToken = default)
        {
            ToDo newTodo = _mapper.Map<CreateToDoDto, ToDo>(todoDto);
            newTodo.CreatedTime = DateTime.UtcNow;
            newTodo.OwnerId = currentUserId;

            await _toDoRepository.AddAsync(newTodo, cancellationToken);

            GetToDoDto newMainToDo = _mapper.Map<ToDo, GetToDoDto>(newTodo);
            Log.Information("ToDo added " + JsonSerializer.Serialize(newMainToDo));
            return newMainToDo;
        }

        public async Task<GetToDoDto> UpdateAsync(int currentUserId, int id, UpdateToDoDto newTodo, CancellationToken cancellationToken = default)
        {
            ToDo? todoForUpdate = await _toDoRepository.SingleOrDefaultAsync(x=>x.Id == id, cancellationToken);
            if (todoForUpdate == null)
            {
                Log.Error($"ToDo with id = {id} not found");
                throw new NotFoundException($"ToDo with id = {id} not found");
            }

            var currentUserRoles = await _rolesRepository.GetListAsync(
                expression: x => x.ApplicationUserId == currentUserId,
                cancellationToken: cancellationToken);
            if (!currentUserRoles.Any(t => t.ApplicationUserRole.Name == "Admin") && currentUserId != todoForUpdate.OwnerId)
            {
                throw new ForbiddenException();
            }

            _mapper.Map<UpdateToDoDto, ToDo>(newTodo, todoForUpdate);
            todoForUpdate.UpdatedTime = DateTime.UtcNow;

            await _toDoRepository.UpdateAsync(todoForUpdate, cancellationToken);

            GetToDoDto updatedMainToDo = _mapper.Map<ToDo, GetToDoDto>(todoForUpdate);
            Log.Information("ToDo updated " + JsonSerializer.Serialize(updatedMainToDo));
            return updatedMainToDo;
        }

        public async Task<IsDoneResult> PatchAsync(int currentUserId, int id, bool isDone, CancellationToken cancellationToken = default)
        {
            ToDo? todo = await _toDoRepository.SingleOrDefaultAsync(x=>x.Id == id, cancellationToken);
            if (todo == null)
            {
                Log.Error($"ToDo with id = {id} not found");
                throw new NotFoundException($"ToDo with id = {id} not found");
            }

            var currentUserRoles = await _rolesRepository.GetListAsync(
                expression: x => x.ApplicationUserId == currentUserId,
                cancellationToken: cancellationToken);
            if (!currentUserRoles.Any(t => t.ApplicationUserRole.Name == "Admin") && currentUserId != todo.OwnerId)
            {
                throw new ForbiddenException();
            }

            todo.IsDone = isDone;
            await _toDoRepository.UpdateAsync(todo, cancellationToken);

            Log.Information("ToDo patched " + JsonSerializer.Serialize(_mapper.Map<ToDo,GetToDoDto>(todo)));
            return new IsDoneResult() {Id = todo.Id, IsDone = todo.IsDone };
        }

        public async Task<bool> DeleteAsync(int currentUserId, int id, CancellationToken cancellationToken)
        {
            ToDo? todoById = await _toDoRepository.SingleOrDefaultAsync(x=>x.Id == id, cancellationToken);
            if (todoById == null)
            {
                Log.Error($"ToDo with id = {id} not found");
                throw new NotFoundException($"ToDo with id = {id} not found");
            }

            var currentUserRoles = await _rolesRepository.GetListAsync(
                expression: x => x.ApplicationUserId == currentUserId,
                cancellationToken: cancellationToken);
            if (!currentUserRoles.Any(t => t.ApplicationUserRole.Name == "Admin") && currentUserId != todoById.OwnerId)
            {
                throw new ForbiddenException();
            }

            await _toDoRepository.DeleteAsync(todoById, cancellationToken);
            Log.Information("ToDo deleted " + JsonSerializer.Serialize(_mapper.Map<ToDo, GetToDoDto>(todoById)));
            return true;
        }

        public async Task<int> GetCountAsync(int currentUserId, int? ownerId, string? labelFreeText, CancellationToken cancellationToken = default)
        {
            var currentUserRoles = await _rolesRepository.GetListAsync(
                expression: x => x.ApplicationUserId == currentUserId,
                cancellationToken: cancellationToken);

            bool isCurrentUserAdmin = currentUserRoles.Any(t => t.ApplicationUserRole.Name == "Admin");

            return await _toDoRepository.CountAsync(
                t => (string.IsNullOrWhiteSpace(labelFreeText) || t.Label.Contains(labelFreeText))
                && (ownerId == null || t.OwnerId == ownerId)
                && (isCurrentUserAdmin || t.OwnerId == currentUserId), cancellationToken);
        }
    }
}

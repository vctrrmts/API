using AutoMapper;
using Common.Application.Abstractions;
using Common.Application.Abstractions.Persistence;
using Common.Application.Exceptions;
using Common.Domain;
using MediatR;
using Serilog;
using System.Text.Json;
using Todos.Application.Dtos;

namespace Todos.Application.Command.UpdateTodo
{
    public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, GetTodoDto>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IRepository<ToDo> _todos;
        private readonly TodosMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public UpdateTodoCommandHandler(
            ICurrentUserService currentUserService,
            IRepository<ToDo> todos,
            TodosMemoryCache memoryCache,
            IMapper mapper) 
        {
            _currentUser = currentUserService;
            _todos = todos;
            _memoryCache = memoryCache;
            _mapper = mapper;
        }
        public async Task<GetTodoDto> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
        {
            ToDo? todoForUpdate = await _todos.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (todoForUpdate == null)
            {
                Log.Error($"ToDo with id = {request.Id} not found");
                throw new NotFoundException($"ToDo with id = {request.Id} not found");
            }

            if (!_currentUser.CurrentUserRoles.Contains("Admin") && _currentUser.CurrentUserId != todoForUpdate.OwnerId)
            {
                throw new ForbiddenException();
            }

            _mapper.Map<UpdateTodoCommand, ToDo>(request, todoForUpdate);
            todoForUpdate.UpdatedTime = DateTime.UtcNow;

            await _todos.UpdateAsync(todoForUpdate, cancellationToken);

            GetTodoDto updatedMainToDo = _mapper.Map<ToDo, GetTodoDto>(todoForUpdate);

            _memoryCache.Cache.Clear();

            Log.Information("ToDo updated " + JsonSerializer.Serialize(updatedMainToDo));
            return updatedMainToDo;
        }
    }
}

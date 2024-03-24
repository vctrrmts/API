using AutoMapper;
using Common.Application.Abstractions;
using Common.Application.Abstractions.Persistence;
using Common.Application.Exceptions;
using Common.Domain;
using MediatR;
using Serilog;
using System.Text.Json;
using Todos.Application.Dtos;

namespace Todos.Application.Command.DeleteTodo
{
    public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IRepository<ToDo> _todos;
        private readonly TodosMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public DeleteTodoCommandHandler(
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

        public async Task Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
        {
            ToDo? todoById = await _todos.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (todoById == null)
            {
                Log.Error($"ToDo with id = {request.Id} not found");
                throw new NotFoundException($"ToDo with id = {request.Id} not found");
            }

            if (!_currentUser.CurrentUserRoles.Contains("Admin") && _currentUser.CurrentUserId != todoById.OwnerId)
            {
                throw new ForbiddenException();
            }

            await _todos.DeleteAsync(todoById, cancellationToken);

            _memoryCache.Cache.Clear();

            Log.Information("ToDo deleted " + JsonSerializer.Serialize(_mapper.Map<ToDo, GetTodoDto>(todoById)));
        }
    }
}

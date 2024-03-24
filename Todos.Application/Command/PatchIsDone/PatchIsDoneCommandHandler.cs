using AutoMapper;
using Common.Application.Abstractions;
using Common.Application.Abstractions.Persistence;
using Common.Application.Exceptions;
using Common.Domain;
using MediatR;
using Serilog;
using System.Text.Json;
using Todos.Application.Dtos;
using Todos.Application.Models;

namespace Todos.Application.Command.PatchIsDone
{
    public class PatchIsDoneCommandHandler : IRequestHandler<PatchIsDoneCommand, IsDoneResult>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IRepository<ToDo> _todos;
        private readonly TodosMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public PatchIsDoneCommandHandler(
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

        public async Task<IsDoneResult> Handle(PatchIsDoneCommand request, CancellationToken cancellationToken)
        {
            ToDo? todo = await _todos.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (todo == null)
            {
                Log.Error($"ToDo with id = {request.Id} not found");
                throw new NotFoundException($"ToDo with id = {request.Id} not found");
            }

            if (!_currentUser.CurrentUserRoles.Contains("Admin") && _currentUser.CurrentUserId != todo.OwnerId)
            {
                throw new ForbiddenException();
            }

            todo.IsDone = request.IsDone;
            await _todos.UpdateAsync(todo, cancellationToken);

            _memoryCache.Cache.Clear();

            Log.Information("ToDo patched " + JsonSerializer.Serialize(_mapper.Map<ToDo, GetTodoDto>(todo)));
            return new IsDoneResult() { Id = todo.Id, IsDone = todo.IsDone };
        }
    }
}

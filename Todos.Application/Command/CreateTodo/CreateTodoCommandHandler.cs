using AutoMapper;
using Common.Application.Abstractions;
using Common.Application.Abstractions.Persistence;
using Common.Domain;
using MediatR;
using Serilog;
using System.Text.Json;
using Todos.Application.Dtos;

namespace Todos.Application.Command.CreateTodo
{
    public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, GetTodoDto>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IRepository<ToDo> _todos;
        private readonly TodosMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public CreateTodoCommandHandler(
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


        public async Task<GetTodoDto> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
        {
            ToDo newTodo = _mapper.Map<CreateTodoCommand, ToDo>(request);
            newTodo.CreatedTime = DateTime.UtcNow;
            newTodo.OwnerId = _currentUser.CurrentUserId;

            await _todos.AddAsync(newTodo, cancellationToken);

            GetTodoDto newMainToDo = _mapper.Map<ToDo, GetTodoDto>(newTodo);

            _memoryCache.Cache.Clear();

            Log.Information("ToDo added " + JsonSerializer.Serialize(newMainToDo));
            return newMainToDo;
        }
    }
}

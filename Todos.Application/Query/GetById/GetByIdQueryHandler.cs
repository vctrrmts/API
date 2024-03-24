using AutoMapper;
using Common.Application.Abstractions;
using Common.Application.Abstractions.Persistence;
using Common.Application.Exceptions;
using Common.Domain;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;
using Todos.Application.Dtos;

namespace Todos.Application.Query.GetById
{
    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, GetTodoDto>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IRepository<ToDo> _todos;
        private readonly MemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public GetByIdQueryHandler(
            ICurrentUserService currentUser,
            IRepository<ToDo> todos,
            TodosMemoryCache memoryCache,
            IMapper mapper) 
        {
            _currentUser = currentUser;
            _todos = todos;
            _memoryCache = memoryCache.Cache;
            _mapper = mapper;
        }

        public async Task<GetTodoDto> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = JsonSerializer.Serialize($"GetById:{request}", new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });

            if (_memoryCache.TryGetValue(cacheKey, out GetTodoDto? result))
            {
                return result!;
            }

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

            result = _mapper.Map<ToDo, GetTodoDto>(todo);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(10))
                .SetSlidingExpiration(TimeSpan.FromSeconds(5))
                .SetSize(2);

            _memoryCache.Set(cacheKey, result, cacheEntryOptions);

            return result;
        }
    }
}

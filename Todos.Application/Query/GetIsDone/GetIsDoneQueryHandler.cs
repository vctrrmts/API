using Common.Application.Abstractions;
using Common.Domain;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System.Text.Json.Serialization;
using System.Text.Json;
using Todos.Application.Models;
using Common.Application.Abstractions.Persistence;
using Common.Application.Exceptions;

namespace Todos.Application.Query.GetIsDone
{
    public class GetIsDoneQueryHandler : IRequestHandler<GetIsDoneQuery, IsDoneResult>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IRepository<ToDo> _todos;
        private readonly MemoryCache _memoryCache;

        public GetIsDoneQueryHandler(
            ICurrentUserService currentUser, 
            IRepository<ToDo> todos, 
            TodosMemoryCache memoryCache)
        {
            _currentUser = currentUser;
            _todos = todos;
            _memoryCache = memoryCache.Cache;
        }

        public async Task<IsDoneResult> Handle(GetIsDoneQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = JsonSerializer.Serialize($"GetIsDone:{request}", new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });

            if (_memoryCache.TryGetValue(cacheKey, out IsDoneResult? result))
            {
                return result!;
            }

            ToDo? todo = await _todos.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken = default);
            if (todo == null)
            {
                Log.Error($"ToDo with id = {request.Id} not found");
                throw new NotFoundException($"ToDo with id = {request.Id} not found");
            }

            if (!_currentUser.CurrentUserRoles.Contains("Admin") && _currentUser.CurrentUserId != todo.OwnerId)
            {
                throw new ForbiddenException();
            }

            result = new IsDoneResult() { Id = request.Id, IsDone = todo.IsDone };

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(10))
                .SetSlidingExpiration(TimeSpan.FromSeconds(5))
                .SetSize(1);

            _memoryCache.Set(cacheKey, result, cacheEntryOptions);

            return result;
        }
    }
}

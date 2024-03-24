using Common.Application.Abstractions;
using Common.Domain;
using MediatR;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Common.Application.Abstractions.Persistence;

namespace Todos.Application.Query.GetCount
{
    public class GetCountQueryHandler : IRequestHandler<GetCountQuery, int>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IRepository<ToDo> _todos;
        private readonly MemoryCache _memoryCache;

        public GetCountQueryHandler(
            ICurrentUserService currentUser, 
            IRepository<ToDo> todos,
            TodosMemoryCache memoryCache)
        {
            _currentUser = currentUser;
            _todos = todos;
            _memoryCache = memoryCache.Cache;
        }

        public async Task<int> Handle(GetCountQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = JsonSerializer.Serialize($"GetCount:{request}", new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });

            if (_memoryCache.TryGetValue(cacheKey, out int? result))
            {
                return result!.Value;
            }

            bool isCurrentUserAdmin = _currentUser.CurrentUserRoles.Contains("Admin");

            result = await _todos.CountAsync(
                t => (string.IsNullOrWhiteSpace(request.LabelFreeText) || t.Label.Contains(request.LabelFreeText))
                && (request.OwnerId == null || t.OwnerId == request.OwnerId)
                && (isCurrentUserAdmin || t.OwnerId == _currentUser.CurrentUserId), cancellationToken);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(10))
                .SetSlidingExpiration(TimeSpan.FromSeconds(5))
                .SetSize(1);

            _memoryCache.Set(cacheKey, result, cacheEntryOptions);

            return result.Value;
        }
    }
}

using Common.Domain;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;
using System.Text.Json;
using Common.Application.Abstractions.Persistence;


namespace Users.Application.Query.GetCount
{
    public class GetCountQueryHandler : IRequestHandler<GetCountQuery, int>
    {
        private readonly IRepository<ApplicationUser> _users;

        private readonly MemoryCache _memoryCache;

        public GetCountQueryHandler(IRepository<ApplicationUser> users, UsersMemoryCache memoryCache) 
        {
            _users = users;
            _memoryCache = memoryCache.Cache;
        }

        public async Task<int> Handle(GetCountQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = JsonSerializer.Serialize($"Count:{request}", new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });

            if (_memoryCache.TryGetValue(cacheKey, out int? result))
            {
                return result!.Value;
            }

            result = await _users.CountAsync(
                t => (string.IsNullOrWhiteSpace(request.NameFreeText) || t.Login.Contains(request.NameFreeText)),
                cancellationToken);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(10))
                .SetSlidingExpiration(TimeSpan.FromSeconds(5))
                .SetSize(1);

            _memoryCache.Set(cacheKey, result, cacheEntryOptions);

            return result.Value;
        }
    }
}

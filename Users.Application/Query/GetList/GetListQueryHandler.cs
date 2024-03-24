using AutoMapper;
using Common.Application.Abstractions.Persistence;
using Common.Domain;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;
using Users.Application.Dtos;

namespace Users.Application.Query.GetList
{
    public class GetListQueryHandler : IRequestHandler<GetListQuery, IReadOnlyCollection<GetUserDto>>
    {
        private readonly IRepository<ApplicationUser> _users;
        private readonly MemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public GetListQueryHandler(IRepository<ApplicationUser> users, UsersMemoryCache memoryCache, IMapper mapper) 
        {
            _users = users;
            _memoryCache  = memoryCache.Cache;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<GetUserDto>> Handle(GetListQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = JsonSerializer.Serialize($"GetList:{request}", new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });

            if (_memoryCache.TryGetValue(cacheKey, out IReadOnlyCollection<GetUserDto>? result))
            {
                return result!;
            }

            result = _mapper.Map<IReadOnlyCollection<GetUserDto>>(await _users.GetListAsync(
            request.Offset,
            request.Limit,
            request.NameFreeText == null ? null : x => x.Login.Contains(request.NameFreeText),
            x => x.Id, false,
            cancellationToken));

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(10))
                .SetSlidingExpiration(TimeSpan.FromSeconds(5))
                .SetSize(3);

            _memoryCache.Set(cacheKey, result, cacheEntryOptions);

            return result;
        }
    }
}

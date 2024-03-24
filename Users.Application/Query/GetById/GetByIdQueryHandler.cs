using AutoMapper;
using Common.Domain;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System.Text.Json.Serialization;
using System.Text.Json;
using Users.Application.Dtos;
using Common.Application.Abstractions.Persistence;
using Common.Application.Exceptions;

namespace Users.Application.Query.GetById
{
    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, GetUserDto>
    {
        private readonly IRepository<ApplicationUser> _users;

        private readonly MemoryCache _memoryCache;

        private readonly IMapper _mapper;

        public GetByIdQueryHandler(IRepository<ApplicationUser> users, UsersMemoryCache memoryCache, IMapper mapper)
        {
            _users = users;
            _memoryCache = memoryCache.Cache;
            _mapper = mapper;
        }

        public async Task<GetUserDto> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = JsonSerializer.Serialize($"GetById:{request}", new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });

            if (_memoryCache.TryGetValue(cacheKey, out GetUserDto? result))
            {
                return result!;
            }

            ApplicationUser? user = await _users.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (user == null)
            {
                Log.Error($"User with Id = {request.Id} not found");
                throw new NotFoundException($"User with Id = {request.Id} not found");
            }

            result = _mapper.Map<ApplicationUser, GetUserDto>(user);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(10))
                .SetSlidingExpiration(TimeSpan.FromSeconds(5))
                .SetSize(2);

            _memoryCache.Set(cacheKey, result, cacheEntryOptions);

            return result;
        }
    }
}

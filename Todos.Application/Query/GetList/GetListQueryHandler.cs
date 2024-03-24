using AutoMapper;
using Common.Application.Abstractions;
using Common.Domain;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;
using System.Text.Json;
using Todos.Application.Dtos;
using Common.Application.Abstractions.Persistence;

namespace Todos.Application.Query.GetList
{
    public class GetListQueryHandler : IRequestHandler<GetListQuery, IReadOnlyCollection<GetTodoDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IRepository<ToDo> _todos;
        private readonly MemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public GetListQueryHandler(
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

        public async Task<IReadOnlyCollection<GetTodoDto>> Handle(GetListQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = JsonSerializer.Serialize($"GetList:{request}", new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });

            if (_memoryCache.TryGetValue(cacheKey, out IReadOnlyCollection<GetTodoDto>? result))
            {
                return result!;
            }

            bool isCurrentUserAdmin = _currentUser.CurrentUserRoles.Contains("Admin");

            result = _mapper.Map<IReadOnlyCollection<GetTodoDto>>(await _todos.GetListAsync(
                request.Offset,
                request.Limit,
                t => (string.IsNullOrWhiteSpace(request.LabelFreeText) || t.Label.Contains(request.LabelFreeText))
                && (request.OwnerId == null || t.OwnerId == request.OwnerId)
                && (isCurrentUserAdmin || t.OwnerId == _currentUser.CurrentUserId),
                x => x.Id,
                false,
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

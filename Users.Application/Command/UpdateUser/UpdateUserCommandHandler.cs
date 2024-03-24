using AutoMapper;
using Common.Application.Abstractions;
using Common.Application.Abstractions.Persistence;
using Common.Application.Exceptions;
using Common.Domain;
using MediatR;
using Serilog;
using System.Text.Json;
using Users.Application.Dtos;

namespace Users.Application.Command.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, GetUserDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<ApplicationUser> _users;
        private readonly IMapper _mapper;
        private readonly UsersMemoryCache _memoryCache;

        public UpdateUserCommandHandler(
            ICurrentUserService currentUserService,
            IRepository<ApplicationUser> users,
            IMapper mapper,
            UsersMemoryCache memoryCache)
        {
            _currentUserService = currentUserService;
            _users = users;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<GetUserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.CurrentUserRoles.Contains("Admin") 
                && _currentUserService.CurrentUserId != request.Id)
            {
                throw new ForbiddenException();
            }

            if (await _users.SingleOrDefaultAsync(x => x.Login == request.Login.Trim()) is not null)
            {
                throw new BadRequestException("User login exist");
            }

            ApplicationUser? user = await _users.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (user == null)
            {
                Log.Error($"User with Id = {request.Id} not found");
                throw new NotFoundException($"User with Id = {request.Id} not found");
            }

            _mapper.Map<UpdateUserCommand, ApplicationUser>(request, user);

            await _users.UpdateAsync(user, cancellationToken);

            _memoryCache.Cache.Clear();

            var getUserDto = _mapper.Map<ApplicationUser, GetUserDto>(user);
            Log.Information("User updated " + JsonSerializer.Serialize(getUserDto));
            return getUserDto;
        }
    }
}

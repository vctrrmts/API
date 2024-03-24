using Common.Application.Abstractions;
using Common.Application.Abstractions.Persistence;
using Common.Application.Exceptions;
using Common.Domain;
using MediatR;
using Serilog;
using System.Text.Json;

namespace Users.Application.Command.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<ApplicationUser> _users;
        private readonly UsersMemoryCache _memoryCache;

        public DeleteUserCommandHandler(
            ICurrentUserService currentUserService,
            IRepository<ApplicationUser> users,
            UsersMemoryCache memoryCache) 
        {
            _currentUserService = currentUserService;
            _users = users;
            _memoryCache = memoryCache;
        }
        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.CurrentUserRoles.Contains("Admin"))
            {
                throw new ForbiddenException();
            }

            var user = await _users.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (user == null)
            {
                Log.Error($"User with Id = {request.Id} not found");
                throw new NotFoundException($"User with Id = {request.Id} not found");
            }

            await _users.DeleteAsync(user, cancellationToken);

            _memoryCache.Cache.Clear();

            Log.Information("User deleted " + JsonSerializer.Serialize(user));
        }
    }
}

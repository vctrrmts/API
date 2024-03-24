using Common.Application.Abstractions;
using Common.Application.Abstractions.Persistence;
using Common.Application.Exceptions;
using Common.Application.Utils;
using Common.Domain;
using MediatR;
using Serilog;

namespace Users.Application.Command.UpdatePassword
{
    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<ApplicationUser> _users;

        public UpdatePasswordCommandHandler(
            ICurrentUserService currentUserService,
            IRepository<ApplicationUser> users) 
        {
            _currentUserService = currentUserService;
            _users = users;
        }

        public async Task Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.CurrentUserRoles.Contains("Admin")
                && _currentUserService.CurrentUserId != request.Id)
            {
                throw new ForbiddenException();
            }

            ApplicationUser? user = await _users.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (user == null)
            {
                Log.Error($"User with Id = {request.Id} not found");
                throw new NotFoundException($"User with Id = {request.Id} not found");
            }

            user.PasswordHash = PasswordHashUtil.HashPassword(request.Password);

            await _users.UpdateAsync(user, cancellationToken);
            Log.Information("User's passord updated. Id user = " + user.Id);
        }
    }
}

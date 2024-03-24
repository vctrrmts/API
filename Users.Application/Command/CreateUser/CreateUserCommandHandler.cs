using AutoMapper;
using Common.Application.Abstractions.Persistence;
using Common.Application.Exceptions;
using Common.Application.Utils;
using Common.Domain;
using MediatR;
using Serilog;
using System.Text.Json;
using Users.Application.Dtos;

namespace Users.Application.Command.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, GetUserDto>
    {
        private readonly IRepository<ApplicationUser> _users;
        private readonly IRepository<ApplicationUserRole> _roles;
        private readonly IMapper _mapper;
        private readonly UsersMemoryCache _memoryCache;

        public CreateUserCommandHandler(
            IRepository<ApplicationUser> users,
            IRepository<ApplicationUserRole> roles,
            IMapper mapper,
            UsersMemoryCache memoryCache) 
        {
            _users = users;
            _roles = roles;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<GetUserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (await _users.SingleOrDefaultAsync(x => x.Login == request.Login.Trim()) is not null)
            {
                throw new BadRequestException("User login exist");
            }

            var userRole = await _roles.SingleAsync(r => r.Name == "Client", cancellationToken);

            var newUser = new ApplicationUser()
            {
                Login = request.Login.Trim(),
                PasswordHash = PasswordHashUtil.HashPassword(request.Password),
                Roles = new[] { new ApplicationUserApplicationRole() { ApplicationUserRoleId = userRole.Id } }
            };

            await _users.AddAsync(newUser, cancellationToken);
            throw new BadRequestException("1234");
            var newUser2 = new ApplicationUser()
            {
                Login = "Valera",
                PasswordHash = PasswordHashUtil.HashPassword(request.Password),
                Roles = new[] { new ApplicationUserApplicationRole() { ApplicationUserRoleId = userRole.Id } }
            };

            await _users.AddAsync(newUser, cancellationToken);

            _memoryCache.Cache.Clear();

            var getUserDto = _mapper.Map<ApplicationUser, GetUserDto>(newUser);
            Log.Information("User added " + JsonSerializer.Serialize(getUserDto));
            return getUserDto;
        }
    }
}

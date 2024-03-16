using AutoMapper;
using Common.Domain;
using Common.Repositories;
using Common.Api.Exceptions;
using Common.Api.Utils;
using Users.Service.Dto;
using Serilog;
using System.Text.Json;
using System.Linq.Expressions;

namespace Users.Service
{
    public class UserService : IUserService
    {
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IRepository<ApplicationUserRole> _rolesRepository;
        private readonly IRepository<ApplicationUserApplicationRole> _userRolesRepository;
        private readonly IMapper _mapper;
        public UserService(
            IRepository<ApplicationUser> userRepository, 
            IRepository<ApplicationUserRole> rolesRepository,
            IRepository<ApplicationUserApplicationRole> userRoles, 
            IMapper mapper)
        {
            _userRepository = userRepository;
            _rolesRepository = rolesRepository;
            _userRolesRepository = userRoles;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<GetUserDto>> GetListAsync(int? offset, string? labelFreeText, 
            int? limit = 5, CancellationToken cancellationToken = default)
        {
            ApplicationUser[] userList = await _userRepository.GetListAsync(offset, 
                limit, 
                labelFreeText == null? null :  x => x.Login.Contains(labelFreeText),
                x=>x.Id, false,
                cancellationToken);

            return _mapper.Map<IReadOnlyCollection<GetUserDto>>(userList);
        }

        public async Task<GetUserDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await _userRepository.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null)
            {
                Log.Error($"User with Id = {id} not found");
                throw new NotFoundException($"User with Id = {id} not found");
            }

            return _mapper.Map<ApplicationUser, GetUserDto>(user);
        }

        public async Task<GetUserDto> CreateAsync(CreateUserDto userDto, CancellationToken cancellationToken = default)
        {
            if (await _userRepository.SingleOrDefaultAsync(x=>x.Login == userDto.Login.Trim()) is not null)
            {
                throw new BadRequestException("User login exist");
            }

            var userRole = await _rolesRepository.SingleAsync(r => r.Name == "Client", cancellationToken);

            var newUser = new ApplicationUser()
            {
                Login = userDto.Login.Trim(),
                PasswordHash = PasswordHashUtil.HashPassword(userDto.Password),
                Roles = new[] { new ApplicationUserApplicationRole() { ApplicationUserRoleId = userRole.Id } }
            };

            await _userRepository.AddAsync(newUser, cancellationToken);

            var getUserDto = _mapper.Map<ApplicationUser, GetUserDto>(newUser);
            Log.Information("User added " + JsonSerializer.Serialize(getUserDto));
            return getUserDto;
        }

        public async Task<GetUserDto> UpdateUserAsync(int currentUserId, int id, UpdateUserDto userDto, CancellationToken cancellationToken = default)
        {
            var currentUserRoles = await _userRolesRepository.GetListAsync(
                expression: x => x.ApplicationUserId == currentUserId,
                cancellationToken: cancellationToken);
            if (!currentUserRoles.Any(t => t.ApplicationUserRole.Name == "Admin") && currentUserId != id)
            {
                throw new ForbiddenException();
            }

            if (await _userRepository.SingleOrDefaultAsync(x => x.Login == userDto.Login.Trim()) is not null)
            {
                throw new BadRequestException("User login exist");
            }

            ApplicationUser? user = await _userRepository.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null)
            {
                Log.Error($"User with Id = {id} not found");
                throw new NotFoundException($"User with Id = {id} not found");
            }

            _mapper.Map<UpdateUserDto, ApplicationUser>(userDto, user);

            await _userRepository.UpdateAsync(user, cancellationToken);

            var getUserDto = _mapper.Map<ApplicationUser, GetUserDto>(user);
            Log.Information("User updated " + JsonSerializer.Serialize(getUserDto));
            return getUserDto;
        }

        public async Task UpdatePasswordAsync(int currentUserId, int id, string newPassword, CancellationToken cancellationToken = default)
        {
            var currentUserRoles = await _userRolesRepository.GetListAsync(
                expression: x => x.ApplicationUserId == currentUserId,
                cancellationToken: cancellationToken);
            if (!currentUserRoles.Any(t => t.ApplicationUserRole.Name == "Admin") && currentUserId != id)
            {
                throw new ForbiddenException();
            }

            ApplicationUser? user = await _userRepository.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null)
            {
                Log.Error($"User with Id = {id} not found");
                throw new NotFoundException($"User with Id = {id} not found");
            }

            user.PasswordHash = PasswordHashUtil.HashPassword(newPassword);

            await _userRepository.UpdateAsync(user, cancellationToken);
            Log.Information("User's passord updated. Id user = " + user.Id);
        }

        public async Task DeleteAsync(int currentUserId, int id, CancellationToken cancellationToken)
        {
            var currentUserRoles = await _userRolesRepository.GetListAsync(
                expression: x => x.ApplicationUserId == currentUserId,
                cancellationToken: cancellationToken);
            if (!currentUserRoles.Any(t => t.ApplicationUserRole.Name == "Admin"))
            {
                throw new ForbiddenException();
            }

            var user = await _userRepository.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null)
            {
                Log.Error($"User with Id = {id} not found");
                throw new NotFoundException($"User with Id = {id} not found");
            }

            await _userRepository.DeleteAsync(user, cancellationToken);
            Log.Information("User deleted " + JsonSerializer.Serialize(user));
        }

        public async Task<int> GetCountAsync(string? labelFreeText, CancellationToken cancellationToken = default)
        {
            return await _userRepository.CountAsync(
                t => (string.IsNullOrWhiteSpace(labelFreeText) || t.Login.Contains(labelFreeText, StringComparison.InvariantCultureIgnoreCase)), 
                cancellationToken);
        }
    }
}

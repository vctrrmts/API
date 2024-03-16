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
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        public UserService(IRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<GetUserDto>> GetListAsync(int? offset, string? labelFreeText, 
            int? limit = 5, CancellationToken cancellationToken = default)
        {
            User[] userList = await _userRepository.GetListAsync(offset, 
                limit, 
                labelFreeText == null? null : new List<Expression<Func<User, bool>>>() { x => x.Login.Contains(labelFreeText) },
                x=>x.Id, false,
                cancellationToken);

            return _mapper.Map<IReadOnlyCollection<GetUserDto>>(userList);
        }

        public async Task<GetUserDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null)
            {
                Log.Error($"User with Id = {id} not found");
                throw new NotFoundException($"User with Id = {id} not found");
            }

            return _mapper.Map<User, GetUserDto>(user);
        }

        public async Task<GetUserDto> CreateAsync(CreateUserDto userDto, CancellationToken cancellationToken = default)
        {
            if (await _userRepository.SingleOrDefaultAsync(x=>x.Login == userDto.Login.Trim()) is not null)
            {
                throw new BadRequestException("User login exist");
            }

            var newUser = new User()
            {
                Login = userDto.Login.Trim(),
                PasswordHash = PasswordHashUtil.HashPassword(userDto.Password),
                UserRoleId = 2
            };

            await _userRepository.AddAsync(newUser, cancellationToken);
            Log.Information("User added " + JsonSerializer.Serialize(newUser));
            return _mapper.Map<User, GetUserDto>(newUser);
        }

        public async Task<GetUserDto> UpdateUserAsync(int currentUserId, int id, UpdateUserDto userDto, CancellationToken cancellationToken = default)
        {
            var currentUser = await _userRepository.SingleAsync(u => u.Id == currentUserId, cancellationToken);
            if (currentUser.UserRoleId != 1 && currentUser.Id != id)
            {
                throw new ForbiddenException();
            }

            User? user = await _userRepository.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null)
            {
                Log.Error($"User with Id = {id} not found");
                throw new NotFoundException($"User with Id = {id} not found");
            }

            _mapper.Map<UpdateUserDto, User>(userDto, user);

            await _userRepository.UpdateAsync(user, cancellationToken);
            Log.Information("User updated " + JsonSerializer.Serialize(user));
            return _mapper.Map<User, GetUserDto>(user);
        }

        public async Task UpdatePasswordAsync(int currentUserId, int id, string newPassword, CancellationToken cancellationToken = default)
        {
            var currentUser = await _userRepository.SingleAsync(u => u.Id == currentUserId, cancellationToken);
            if (currentUser.UserRoleId != 1 && currentUser.Id != id)
            {
                throw new ForbiddenException();
            }

            User? user = await _userRepository.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null)
            {
                Log.Error($"User with Id = {id} not found");
                throw new NotFoundException($"User with Id = {id} not found");
            }

            user.PasswordHash = PasswordHashUtil.HashPassword(newPassword);

            await _userRepository.UpdateAsync(user, cancellationToken);
            Log.Information("User updated " + JsonSerializer.Serialize(user));
        }

        public async Task DeleteAsync(int currentUserId, int id, CancellationToken cancellationToken)
        {
            var currentUser = await _userRepository.SingleAsync(u => u.Id == currentUserId, cancellationToken);
            if (currentUser.UserRoleId != 1)
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
            var expressions = new List<Expression<Func<User, bool>>>();
            if (labelFreeText != null)
            {
                expressions.Add(x => x.Login.Contains(labelFreeText));
            }

            return await _userRepository.CountAsync(expressions, cancellationToken);
        }
    }
}

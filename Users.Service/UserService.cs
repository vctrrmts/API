using AutoMapper;
using Common.Domain;
using Common.Repositories;
using Common.Api.Exceptions;
using Users.Service.Dto;
using Serilog;
using System.Text.Json;

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

            if(_userRepository.GetList().Length == 0)
            {
                _userRepository.Add(new User() { Id = 1, Name = "Viktor" });
                _userRepository.Add(new User() { Id = 2, Name = "Igor" });
                _userRepository.Add(new User() { Id = 3, Name = "Gennadiy" });
            }

        }

        public IReadOnlyCollection<MainUserDto> GetList(int? offset, string? labelFreeText, int? limit = 5)
        {
            User[] userList = _userRepository.GetList(offset, 
                limit, 
                labelFreeText == null? null : x=>x.Name.Contains(labelFreeText),
                x=>x.Id);

            MainUserDto[] mainUserList = new MainUserDto[userList.Length];
            for (int i = 0; i < userList.Length; i++)
            {
                mainUserList[i] = _mapper.Map<User, MainUserDto>(userList[i]);
            }

            return mainUserList;
        }

        public async Task<MainUserDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null)
            {
                Log.Error($"User with Id = {id} not found");
                throw new NotFoundException($"User with Id = {id} not found");
            }

            return _mapper.Map<User, MainUserDto>(user);
        }

        public MainUserDto Create(CreateUserDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Name))
            {
                Log.Error("Name must not be empty");
                throw new Exception("Name must not be empty");
            }

            User newUser = _mapper.Map<CreateUserDto, User>(userDto);

            _userRepository.Add(newUser);
            Log.Information("User added " + JsonSerializer.Serialize(newUser));
            return _mapper.Map<User, MainUserDto>(newUser);
        }

        public MainUserDto Update(int id, UpdateUserDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Name))
            {
                Log.Error("Name must not be empty");
                throw new Exception("Name must not be empty");
            }

            User? user = _userRepository.SingleOrDefault(x => x.Id == id);
            if (user == null)
            {
                Log.Error($"User with Id = {id} not found");
                throw new NotFoundException($"User with Id = {id} not found");
            }

            _mapper.Map<UpdateUserDto, User>(userDto, user);

            _userRepository.Update(user);
            Log.Information("User updated " + JsonSerializer.Serialize(user));
            return _mapper.Map<User, MainUserDto>(user);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null)
            {
                Log.Error($"User with Id = {id} not found");
                throw new NotFoundException($"User with Id = {id} not found");
            }

            _userRepository.Delete(user);
            Log.Information("User deleted " + JsonSerializer.Serialize(user));
            return true;
        }

        public int GetCount(string? labelFreeText)
        {
            return _userRepository.Count(labelFreeText == null ? null : x => x.Name.Contains(labelFreeText));
        }
    }
}

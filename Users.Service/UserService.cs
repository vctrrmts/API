using AutoMapper;
using Common.Domain;
using Common.Repositories;
using Common.Application;
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

        public IReadOnlyCollection<User> GetList(int? offset, string? labelFreeText, int? limit = 5)
        {
            return _userRepository.GetList(offset, 
                limit, 
                labelFreeText == null? null : x=>x.Name.Contains(labelFreeText, StringComparison.InvariantCultureIgnoreCase),
                x=>x.Id);
        }

        public User? GetById(int id)
        {
            User? user = _userRepository.SingleOrDefault(x=>x.Id == id);
            if (user == null)
            {
                Log.Error("User not found");
                throw new NotFoundException();
            }

            return user;
        }

        public User Create(CreateUserDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Name))
            {
                Log.Error("Name must not be empty");
                throw new Exception("Name must not be empty");
            }

            User newUser = _mapper.Map<CreateUserDto, User>(userDto);
            newUser.Id = _userRepository.GetList().Length == 0 ? 1 : _userRepository.GetList().Max(x=>x.Id) + 1;

            _userRepository.Add(newUser);
            Log.Information("User added " + JsonSerializer.Serialize(newUser));
            return newUser;
        }

        public User? Update(int id, UpdateUserDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Name))
            {
                Log.Error("Name must not be empty");
                throw new Exception("Name must not be empty");
            }

            User? user = _userRepository.SingleOrDefault(x => x.Id == id);
            if (user == null)
            {
                Log.Error("User not found");
                throw new NotFoundException();
            }

            _mapper.Map<UpdateUserDto, User>(userDto, user);

            _userRepository.Update(user);
            Log.Information("User updated " + JsonSerializer.Serialize(user));
            return user;
        }

        public bool Delete(int id)
        {
            var user = _userRepository.SingleOrDefault(x=>x.Id == id);
            if (user == null)
            {
                Log.Error("User not found");
                throw new NotFoundException();
            }

            _userRepository.Delete(user);
            Log.Information("User deleted " + JsonSerializer.Serialize(user));
            return true;
        }

        public int GetCount(string? labelFreeText)
        {
            return _userRepository.Count(labelFreeText == null ? null : x => x.Name.Contains(labelFreeText, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}

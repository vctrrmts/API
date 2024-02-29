using Common.Domain;
using Common.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IReadOnlyCollection<User> GetList(int? offset, int limit)
        {
            return _userRepository.GetList(offset, limit);
        }

        public User? GetById(int id)
        {
            return _userRepository.GetById(id);
        }

        public User Post(User user)
        {
            return _userRepository.Post(user);
        }

        public User? Patch(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new Exception("Name must not be empty");
            if (_userRepository.GetById(id) == null) throw new Exception("Id does not exist");

            return _userRepository.Patch(id, name);
        }

        public bool Delete(int id)
        {
            User? user = _userRepository.GetById(id);
            if (user == null) return false;
            return _userRepository.Delete(user);
        }
    }
}

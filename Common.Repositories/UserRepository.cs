using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Common.Domain;

namespace Common.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static List<User> userList = new List<User>()
        { new User(){ Id = 1, Name = "Viktor" },
          new User(){ Id = 2, Name = "Igor" },
          new User(){ Id = 3, Name = "Gennadiy" }};

        public IReadOnlyCollection<User> GetList(int? offset, int limit)
        {
            IEnumerable<User> users = userList.OrderBy(x=>x.Id);

            if (offset.HasValue)
            {
                users = users.Skip(offset.Value);
            }

            return users.Take(limit).ToList();
        }

        public User? GetById(int id)
        {
            User result = userList.SingleOrDefault(x => x.Id == id)!;
            return result;
        }

        public User Post(User user)
        {
            int idNew = userList.Max(x => x.Id) + 1;
            User newUser = new User { Id = idNew, Name = user.Name };
            userList.Add(newUser);
            return newUser;
        }

        public User? Patch(int id, string name)
        {
            User user = userList.Single(x => x.Id == id)!;
            user.Name = name;
            return user;
        }

        public bool Delete(User user)
        {
            userList.Remove(user);
            return true;
        }

    }
}

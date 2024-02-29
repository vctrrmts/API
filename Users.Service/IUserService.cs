using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Domain;

namespace Users.Service
{
    public interface IUserService
    {
        IReadOnlyCollection<User> GetList(int? offset, int limit);
        User? GetById(int id);
        User Post(User todo);
        User? Patch(int id, string name);
        bool Delete(int id);
    }
}

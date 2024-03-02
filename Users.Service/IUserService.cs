using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Domain;
using Users.Service.Dto;

namespace Users.Service
{
    public interface IUserService
    {
        IReadOnlyCollection<User> GetList(int? offset, string? labelFreeText, int? limit);
        User? GetById(int id);
        User Create(CreateUserDto user);
        User? Update(int id, UpdateUserDto user);
        bool Delete(int id);
        int GetCount(string? labelFreeText);
    }
}

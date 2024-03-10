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
        IReadOnlyCollection<MainUserDto> GetList(int? offset, string? labelFreeText, int? limit);
        Task<MainUserDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        MainUserDto Create(CreateUserDto user);
        MainUserDto Update(int id, UpdateUserDto user);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        int GetCount(string? labelFreeText);
    }
}

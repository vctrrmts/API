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
        Task<IReadOnlyCollection<GetUserDto>> GetListAsync(int? offset, string? labelFreeText, int? limit, CancellationToken cancellationToken = default);
        Task<GetUserDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<GetUserDto> CreateAsync(CreateUserDto user, CancellationToken cancellationToken = default);
        Task<GetUserDto> UpdateUserAsync(int currentUserId, int id, UpdateUserDto user, CancellationToken cancellationToken = default);
        Task UpdatePasswordAsync(int currentUserId, int id, string NewPassword, CancellationToken cancellationToken = default);
        Task DeleteAsync(int currentUserId, int id, CancellationToken cancellationToken = default);
        Task<int> GetCountAsync(string? labelFreeText, CancellationToken cancellationToken = default);
    }
}

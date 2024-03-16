using Common.Domain;
using Todos.Service.Dto;
using Todos.Service.Models;

namespace Todos.Service
{
    public interface IToDoService
    {
        Task<IReadOnlyCollection<GetToDoDto>> GetListAsync(int currentUserId, int? offset, int? ownerId, 
            string? labelFreeText, int? limit, CancellationToken cancellationToken = default);
        Task<GetToDoDto> GetByIdAsync(int currentUserId, int id, CancellationToken cancellationToken = default);
        Task<IsDoneResult> GetIsDoneAsync(int currentUserId, int id, CancellationToken cancellationToken);
        Task<GetToDoDto> CreateAsync(int currentUserId, CreateToDoDto todo, CancellationToken cancellationToken = default);
        Task<GetToDoDto> UpdateAsync(int currentUserId, int id, UpdateToDoDto todo, CancellationToken cancellationToken = default);
        Task<IsDoneResult> PatchAsync(int currentUserId, int id, bool isDone, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int currentUserId, int id, CancellationToken cancellationToken = default);
        Task<int> GetCountAsync(int currentUserId, int? ownerId, string? labelFreeText, CancellationToken cancellationToken = default);
    }
}

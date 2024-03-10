using Common.Domain;
using Todos.Service.Dto;
using Todos.Service.Models;

namespace Todos.Service
{
    public interface IToDoService
    {
        IReadOnlyCollection<MainToDoDto> GetList(int? offset, int? ownerId, string? labelFreeText, int? limit);
        Task<MainToDoDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IsDoneResult> GetIsDoneAsync(int id, CancellationToken cancellationToken);
        MainToDoDto Create(CreateToDoDto todo);
        MainToDoDto Update(int id, UpdateToDoDto todo);
        IsDoneResult Patch(int id, bool isDone);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        int GetCount(string? labelFreeText);
    }
}

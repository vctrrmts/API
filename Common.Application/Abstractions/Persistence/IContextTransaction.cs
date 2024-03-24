namespace Common.Application.Abstractions.Persistence
{
    public interface IContextTransaction : IDisposable, IAsyncDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken);
        Task RollbackAsync(CancellationToken cancellationToken);
    }
}

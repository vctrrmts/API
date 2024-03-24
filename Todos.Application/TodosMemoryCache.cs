using Microsoft.Extensions.Caching.Memory;

namespace Todos.Application
{
    public class TodosMemoryCache
    {
        public MemoryCache Cache { get; } = new MemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = 1024,
                ExpirationScanFrequency = new TimeSpan(0, 0, 3)
            });
    }
}

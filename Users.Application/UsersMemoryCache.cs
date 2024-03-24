using Microsoft.Extensions.Caching.Memory;

namespace Users.Application
{
    public class UsersMemoryCache
    {
        public MemoryCache Cache { get; } = new MemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = 1024,
                ExpirationScanFrequency = new TimeSpan(0, 0, 3)
            });
    }
}

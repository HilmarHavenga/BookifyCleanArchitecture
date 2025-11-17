namespace Bookify.Infrastructure.Caching;

public static class CacheEntryOptions
{
    public static DistributedCacheEntryOptions DefaultExpiration => new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
    };

    public static DistributedCacheEntryOptions Create(TimeSpan? expiration) =>
        expiration is not null ? 
        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration } 
        : DefaultExpiration;
}
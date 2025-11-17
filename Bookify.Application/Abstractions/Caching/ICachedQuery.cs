namespace Bookify.Application.Abstractions.Caching;

public interface ICachedQuery<TResponse> : IQuery<TResponse>, ICachedQuery;

//Marker interface
public interface ICachedQuery
{
    string CacheKey { get; }

    TimeSpan? Expiration { get; }
}
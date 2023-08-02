using System.Diagnostics.CodeAnalysis;

namespace TodoApi.Common.Caching;

public interface IRedisCache
{
    Task<TValue?> GetCacheValueAsync<TValue>(string key);
    
    Task<TValue?> GetOrSetCacheValueAsync<TValue>(
        string key, 
        Func<Task<TValue>> factory);
    
    Task SetCacheValueAsync<TValue>(
        string key, 
        [DisallowNull] TValue value);
    
    Task SetCacheValueAsync<TValue>(
        string key, 
        [DisallowNull] TValue value, 
        TimeSpan absoluteExpirationRelativeToNow);

    Task SetCacheValueAsync<TValue>(
        string key, 
        [DisallowNull] TValue value, 
        DateTimeOffset absoluteExpiration);

    Task RemoveCacheValueAsync(string key);

    Task<TValue?> GetOrSetCacheValueAsync<TValue>(
        string key,
        Func<Task<TValue>> factory,
        TimeSpan absoluteExpirationRelativeToNow);
}
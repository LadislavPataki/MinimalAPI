using System.Diagnostics.CodeAnalysis;
using StackExchange.Redis;
using TodoApi.Common.DateTime;
using TodoApi.Common.Serialization;

namespace TodoApi.Common.Caching;

public class RedisCache : IRedisCache
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IDatabase _database;

    public RedisCache(
        IConnectionMultiplexer connectionMultiplexer,
        IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<TValue?> GetCacheValueAsync<TValue>(string key)
    {
        if (key == null) 
            throw new ArgumentNullException(nameof(key));
        
        var value = await _database.StringGetAsync(key);

        if (value.IsNull) 
            return default;

        var serializedValue = value.ToString();
        return serializedValue.Deserialize<TValue>();
    }

    public async Task<TValue?> GetOrSetCacheValueAsync<TValue>(string key, Func<Task<TValue>> factory)
    {
        if (key == null) 
            throw new ArgumentNullException(nameof(key));

        var value = await GetCacheValueAsync<TValue>(key);

        if (value is not null)
            return value;

        value = await factory();

        await SetCacheValueAsync(key, value ?? throw new InvalidOperationException());

        return value;
    }

    public async Task<TValue?> GetOrSetCacheValueAsync<TValue>(
        string key,
        Func<Task<TValue>> factory,
        TimeSpan absoluteExpirationRelativeToNow)
    {
        var value = await GetCacheValueAsync<TValue>(key);

        if (value is not null)
            return value;
        
        value = await factory();
        
        await SetCacheValueAsync(
            key, 
            value ?? throw new InvalidOperationException(),
            absoluteExpirationRelativeToNow);

        return value;
    }


    public async Task SetCacheValueAsync<TValue>(string key, [DisallowNull] TValue value)
    {
        if (key == null) 
            throw new ArgumentNullException(nameof(key));
        
        if (value == null) 
            throw new ArgumentNullException(nameof(value));

        await _database.StringSetAsync(key, value.Serialize());
    }

    public async Task SetCacheValueAsync<TValue>(
        string key, 
        [DisallowNull] TValue value, 
        TimeSpan absoluteExpirationRelativeToNow)
    {
        // refactor to use RedisCacheOptions class where we can set options such as Expiration, etc.
        
        if (key == null) 
            throw new ArgumentNullException(nameof(key));
        
        if (value == null) 
            throw new ArgumentNullException(nameof(value));

        await _database.StringSetAsync(key, value.Serialize(), absoluteExpirationRelativeToNow);
    }

    public async Task SetCacheValueAsync<TValue>(
        string key, 
        [DisallowNull] TValue value, 
        DateTimeOffset absoluteExpiration)
    {
        if (key == null) 
            throw new ArgumentNullException(nameof(key));
        
        if (value == null) 
            throw new ArgumentNullException(nameof(value));

        var absoluteExpirationRelativeToNow = absoluteExpiration.UtcDateTime - _dateTimeProvider.UtcNow;
        await _database.StringSetAsync(key, value.Serialize(), absoluteExpirationRelativeToNow);
    }

    public async Task UpdateCacheValueAsync<TValue>(
        string key,
        TValue value)
    {
        if (key == null) 
            throw new ArgumentNullException(nameof(key));
        
        if (value == null) 
            throw new ArgumentNullException(nameof(value));

        await SetCacheValueAsync(key, value);
    }

    public async Task RemoveCacheValueAsync(string key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        
        await _database.KeyDeleteAsync(key);
    }
}
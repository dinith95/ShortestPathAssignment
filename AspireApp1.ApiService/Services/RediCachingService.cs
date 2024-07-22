using AspireApp1.ApiService.Dto;
using StackExchange.Redis;
using System.Text.Json;

namespace AspireApp1.ApiService.Services;

public interface IRediCachingService
{
    Task AddToRedisCache(string source, string dest, DistanceDto dto);
    Task<DistanceDto> CheckInRedisCache(string source, string dest);
}

public class RediCachingService : IRediCachingService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RediCachingService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<DistanceDto> CheckInRedisCache(string source, string dest)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var cacheKey = $"{source}-{dest}";
        var cacheValue = await db.StringGetAsync(cacheKey);
        if (cacheValue.HasValue)
            return JsonSerializer.Deserialize<DistanceDto>(cacheValue);
        else
            return null;
    }

    public async Task AddToRedisCache(string source, string dest, DistanceDto dto)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var cacheKey = $"{source}-{dest}";
        var dtoJson = JsonSerializer.Serialize(dto);
        await db.StringSetAsync(cacheKey, dtoJson);
    }
}

using Microsoft.Extensions.Caching.Memory;

namespace otp_service_template.Services;

public class CacheService
{
  private readonly MemoryCache Cache;
  private readonly MemoryCacheEntryOptions _options;

  public CacheService()
  {
    Cache = Cache = new MemoryCache(new MemoryCacheOptions
    {
      SizeLimit = 1_000
    });

    _options = new MemoryCacheEntryOptions
    {
      SlidingExpiration = TimeSpan.FromSeconds(60),
      AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
      Size = 1,
    };
  }

  public void Save(string key, string value)
  {
    Cache.Set(key, value, _options);
  }

  public string Get(string key)
  {
    return Cache.Get<string>(key)?? "";
  }

  public void Compact()
  {
    Cache.Compact(0.4);
  }
}
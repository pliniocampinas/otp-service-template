using Microsoft.Extensions.Caching.Memory;

namespace otp_service_template.Services.Cache;

public class GetCacheWithLock<TEntry,TParameters> 
{
  private readonly MemoryCache Cache;
  private readonly SemaphoreSlim _cacheLock;
  private readonly ICachableService<TEntry, TParameters> _service;

  public GetCacheWithLock(MyMemoryCache cache, ICachableService<TEntry,TParameters> service)
  {
    Cache = cache.Cache;
    _cacheLock = new SemaphoreSlim(1);
    _service = service;
  }

  public async Task<TEntry> GetCached(TParameters parameters)
  {
    var cacheKey = _service.GetKey(parameters);

    var cachedEntry = Cache.Get<TEntry>(cacheKey);
    if (cachedEntry != null)
    {
      return cachedEntry;
    }

    // Acquire the cache lock to prevent cache stampede
    await _cacheLock.WaitAsync();

    try
    {
      // Check if another request has already updated the cache
      cachedEntry = Cache.Get<TEntry>(cacheKey);
      if (cachedEntry != null)
      {
        return cachedEntry;
      }

      // Retrieve from the data source
      var entryFromSource = await _service.GetFromSource(parameters);

      // Cache the entry
      Cache.Set(cacheKey, entryFromSource, _service.GetEntryOptions());

      return entryFromSource;
    }
    finally
    {
      // Release the cache lock
      _cacheLock.Release();
    }
  }
}
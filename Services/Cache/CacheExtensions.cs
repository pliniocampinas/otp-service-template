

namespace otp_service_template.Services.Cache;

public static class CacheExtensions
{
  public static void AddCachedService<TService,TEntry,TParameters>(this IServiceCollection services) 
    where TService : ICachableService<TEntry,TParameters>
  {
    services.AddSingleton((provider) =>
    {
      var memoryCache = provider.GetService<MyMemoryCache>() ?? throw new Exception($"Failed to resolve MemoryCache");
      var service = provider.GetService<TService>() ?? throw new Exception($"Failed to resolve {typeof(TService).FullName}");

      return new GetCacheWithLock<TEntry, TParameters>(memoryCache, service);
    });
  }
}
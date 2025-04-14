
using Microsoft.Extensions.Caching.Memory;

namespace otp_service_template.Services.Cache;
public interface ICachableService<TEntry,TParameters>
{
  Task<TEntry> GetFromSource(TParameters parameters);
  string GetKey(TParameters parameters);
  MemoryCacheEntryOptions GetEntryOptions();
}
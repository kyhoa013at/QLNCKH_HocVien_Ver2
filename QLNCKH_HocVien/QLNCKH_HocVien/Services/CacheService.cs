using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace QLNCKH_HocVien.Services
{
    /// <summary>
    /// Memory Cache Service Implementation
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;
        private readonly HashSet<string> _cacheKeys;
        private readonly object _lock = new();

        // Default cache duration
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(30);

        public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
            _cacheKeys = new HashSet<string>();
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out T? value))
            {
                _logger.LogDebug("Cache HIT: {Key}", key);
                return Task.FromResult(value);
            }

            _logger.LogDebug("Cache MISS: {Key}", key);
            return Task.FromResult(default(T?));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration,
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };

            // Register removal callback
            options.RegisterPostEvictionCallback((evictedKey, evictedValue, reason, state) =>
            {
                lock (_lock)
                {
                    _cacheKeys.Remove(evictedKey.ToString() ?? "");
                }
                _logger.LogDebug("Cache EVICTED: {Key}, Reason: {Reason}", evictedKey, reason);
            });

            _cache.Set(key, value, options);

            lock (_lock)
            {
                _cacheKeys.Add(key);
            }

            _logger.LogDebug("Cache SET: {Key}, Expiration: {Expiration}", key, expiration ?? DefaultExpiration);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);

            lock (_lock)
            {
                _cacheKeys.Remove(key);
            }

            _logger.LogDebug("Cache REMOVE: {Key}", key);
            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefix)
        {
            List<string> keysToRemove;

            lock (_lock)
            {
                keysToRemove = _cacheKeys.Where(k => k.StartsWith(prefix)).ToList();
            }

            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                lock (_lock)
                {
                    _cacheKeys.Remove(key);
                }
            }

            _logger.LogDebug("Cache REMOVE by prefix: {Prefix}, Count: {Count}", prefix, keysToRemove.Count);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Cache Keys constants
    /// </summary>
    public static class CacheKeys
    {
        public const string AllSinhViens = "sinhviens:all";
        public const string AllGiaoViens = "giaoviens:all";
        public const string AllChuyenDes = "chuyendes:all";
        public const string AllHoiDongs = "hoidongs:all";
        public const string AllXepGiais = "xepgiais:all";

        public static string SinhVien(int id) => $"sinhvien:{id}";
        public static string GiaoVien(int id) => $"giaovien:{id}";
        public static string ChuyenDe(int id) => $"chuyende:{id}";
        public static string HoiDong(int id) => $"hoidong:{id}";

        // Prefixes for invalidation
        public const string SinhVienPrefix = "sinhvien";
        public const string GiaoVienPrefix = "giaovien";
        public const string ChuyenDePrefix = "chuyende";
        public const string HoiDongPrefix = "hoidong";
        public const string XepGiaiPrefix = "xepgiai";
    }
}


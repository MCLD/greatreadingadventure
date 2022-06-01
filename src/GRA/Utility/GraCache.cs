using System;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace GRA.Utility
{
    public class GraCache : Abstract.IGraCache
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<GraCache> _logger;

        public GraCache(ILogger<GraCache> logger, IDistributedCache cache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public Task<bool?> GetBoolFromCacheAsync(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }
            return GetBoolFromCacheInternalAsync(cacheKey);
        }

        public Task<int?> GetIntFromCacheAsync(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }
            return GetIntFromCacheInternalAsync(cacheKey);
        }

        public Task<long?> GetLongFromCacheAsync(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }
            return GetLongFromCacheInternalAsync(cacheKey);
        }

        public async Task<T> GetObjectFromCacheAsync<T>(string cacheKey) where T : class
        {
            var cachedJson = await GetStringFromCache(cacheKey);

            if (!string.IsNullOrEmpty(cachedJson))
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(cachedJson);
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex,
                        "Error deserializing {Type} with key {CacheKey} from cache: {ErrorMessage}",
                        typeof(T),
                        cacheKey,
                        ex.Message);
                    await _cache.RemoveAsync(cacheKey);
                }
            }
            return null;
        }

        public Task<string> GetStringFromCache(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }
            return GetStringFromCacheInternalAsync(cacheKey);
        }

        public async Task RemoveAsync(string cacheKey)
        {
            if (!string.IsNullOrEmpty(cacheKey))
            {
                _logger.LogTrace("Removing cache key {CacheKey} upon request", cacheKey);
                await _cache.RemoveAsync(cacheKey);
            }
        }

        public async Task SaveToCacheAsync<T>(string cacheKey,
            T item,
            int cacheForHours)
        {
            if (cacheForHours > 0)
            {
                await SaveToCacheInternalAsync(cacheKey,
                    item,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(cacheForHours)
                    });
            }
            else
            {
                _logger.LogWarning("Refusing to cache item {CacheKey} for less than {CacheForHours} hour(s)",
                    cacheKey,
                    cacheForHours);
            }
        }

        public async Task SaveToCacheAsync<T>(string cacheKey,
            T item,
            TimeSpan absoluteExpiration)
        {
            await SaveToCacheInternalAsync(cacheKey, item, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration
            });
        }

        public async Task SaveToCacheAsync<T>(string cacheKey,
            T item,
            TimeSpan? absoluteExpiration,
            TimeSpan slidingExpiration)
        {
            await SaveToCacheInternalAsync(cacheKey, item, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration,
                SlidingExpiration = slidingExpiration
            });
        }

        private async Task<bool?> GetBoolFromCacheInternalAsync(string cacheKey)
        {
            var cachedValue = await _cache.GetAsync(cacheKey);

            if (cachedValue?.Length > 0)
            {
                try
                {
                    return BitConverter.ToBoolean(cachedValue);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    _logger.LogWarning(ex,
                        "Error converting bool with key {CacheKey} from cache: {ErrorMessage}",
                        cacheKey,
                        ex.Message);
                    await _cache.RemoveAsync(cacheKey);
                }
            }
            return null;
        }

        private async Task<int?> GetIntFromCacheInternalAsync(string cacheKey)
        {
            var cachedValue = await _cache.GetAsync(cacheKey);

            if (cachedValue?.Length > 0)
            {
                try
                {
                    return BitConverter.ToInt32(cachedValue);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    _logger.LogWarning(ex,
                        "Error converting int with key {CacheKey} from cache: {ErrorMessage}",
                        cacheKey,
                        ex.Message);
                    await _cache.RemoveAsync(cacheKey);
                }
            }
            return null;
        }

        private async Task<long?> GetLongFromCacheInternalAsync(string cacheKey)
        {
            var cachedValue = await _cache.GetAsync(cacheKey);

            if (cachedValue?.Length > 0)
            {
                try
                {
                    return BitConverter.ToInt64(cachedValue);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    _logger.LogWarning(ex,
                        "Error converting long with key {CacheKey} from cache: {ErrorMessage}",
                        cacheKey,
                        ex.Message);
                    await _cache.RemoveAsync(cacheKey);
                }
            }
            return null;
        }

        private async Task<string> GetStringFromCacheInternalAsync(string cacheKey)
        {
            return await _cache.GetStringAsync(cacheKey);
        }

        private Task SaveToCacheInternalAsync<T>(string cacheKey,
            T item,
            DistributedCacheEntryOptions cacheEntryOptions)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }
            if (item == null)
            {
                _logger.LogError("Ignoring attempt to cache null object with key {CacheKey}",
                    cacheKey);
                return Task.CompletedTask;
            }
            if (cacheEntryOptions == null)
            {
                throw new ArgumentNullException(nameof(cacheEntryOptions));
            }
            return SaveToCacheInternalExecuteAsync(cacheKey, item, cacheEntryOptions);
        }

        private async Task SaveToCacheInternalExecuteAsync<T>(string cacheKey,
            T item,
            DistributedCacheEntryOptions cacheEntryOptions)
        {
            int length;
            string description;

            if (item is bool boolValue)
            {
                var bytes = BitConverter.GetBytes(boolValue);
                await _cache.SetAsync(cacheKey, bytes, cacheEntryOptions);
                description = bytes.ToString();
                length = bytes.Length;
            }
            else if (item is int intValue)
            {
                var bytes = BitConverter.GetBytes(intValue);
                await _cache.SetAsync(cacheKey, bytes, cacheEntryOptions);
                description = "bytes " + BitConverter.ToString(bytes);
                length = bytes.Length;
            }
            else if (item is long longValue)
            {
                var bytes = BitConverter.GetBytes(longValue);
                await _cache.SetAsync(cacheKey, bytes, cacheEntryOptions);
                description = "bytes " + BitConverter.ToString(bytes);
                length = bytes.Length;
            }
            else
            {
                string cacheValue;
                if (item is string stringValue)
                {
                    cacheValue = stringValue;
                    description = "string";
                }
                else
                {
                    cacheValue = JsonSerializer.Serialize(item);
                    description = typeof(T).Name;
                }
                length = cacheValue.Length;
                await _cache.SetStringAsync(cacheKey, cacheValue, cacheEntryOptions);
            }

            string timeSpan
                = cacheEntryOptions.AbsoluteExpirationRelativeToNow?.ToString()
                ?? cacheEntryOptions.AbsoluteExpiration?.ToString(CultureInfo.InvariantCulture)
                ?? "unspecified timespan";

            string timespanExpiration = cacheEntryOptions.SlidingExpiration.HasValue
                ? cacheEntryOptions.SlidingExpiration.Value + " sliding"
                : "absolute";

            _logger.LogDebug("Cache miss for {CacheKey}: caching {Description} (length {Length}) expires {CacheTimeSpan} ({ExpirationType})",
                cacheKey,
                description,
                length,
                timeSpan,
                timespanExpiration);
        }
    }
}

using System;
using GRA.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service.Abstract
{
    public abstract class BaseService<TService>
    {
        protected readonly IDateTimeProvider _dateTimeProvider;
        protected readonly ILogger<TService> _logger;

        protected BaseService(ILogger<TService> logger,
            IDateTimeProvider dateTimeProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dateTimeProvider = dateTimeProvider
                ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        protected static string GetCacheKey(string cacheKey, params object[] cacheKeyValues)
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                cacheKey,
                cacheKeyValues);
        }

        protected static int GetPercent(int count, int total)
        {
            return count * 100 / total;
        }
    }
}
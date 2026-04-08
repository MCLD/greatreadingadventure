using System;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service.Abstract
{
    public abstract class BaseService<TService>(ILogger<TService> logger,
        IDateTimeProvider dateTimeProvider)
    {
        protected readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider
            ?? throw new ArgumentNullException(nameof(dateTimeProvider));

        protected readonly ILogger<TService> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

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

        protected static async Task ReportJobStatusAsync(IJobRepository repo,
            JobMetadata metadata,
            JobStatus status)
        {
            ArgumentNullException.ThrowIfNull(repo);
            ArgumentNullException.ThrowIfNull(metadata);
            ArgumentNullException.ThrowIfNull(status);

            await repo.UpdateStatusAsync(metadata.JobId, status.Status);
            metadata.Progress.Report(status);
        }
    }
}

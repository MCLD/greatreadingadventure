using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GRA.Web
{
    internal class JobBackgroundService : BackgroundService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<JobBackgroundService> _logger;
        private readonly IServiceProvider _services;

        private readonly string InstanceName;
        private readonly bool JobsEnabled;
        private readonly int JobSleepSeconds;

        public JobBackgroundService(
            IConfiguration config,
            IDistributedCache cache,
            ILogger<JobBackgroundService> logger,
            IServiceProvider services)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(cache);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(services);

            _cache = cache;
            _logger = logger;
            _services = services;

            InstanceName = string.IsNullOrEmpty(config[ConfigurationKey.InstanceName])
                ? "n/a"
                : config[ConfigurationKey.InstanceName];

            JobsEnabled = int.TryParse(config[ConfigurationKey.JobSleepSeconds], out JobSleepSeconds);

            _logger.LogInformation("Job system {ConfiguredOrNot} to run every {JobSleepSeconds} seconds in instance {InstanceName}",
                JobsEnabled ? "configured" : "not configured",
                JobSleepSeconds,
                InstanceName);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Job system shutdown request entered for {InstanceName}",
                InstanceName);

            await _cache.SetStringAsync(string.Format(CultureInfo.InvariantCulture,
                    CacheKey.JobsStop,
                    InstanceName),
                InstanceName,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                },
                cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!JobsEnabled)
            {
                return;
            }

            await _cache.RemoveAsync(string.Format(CultureInfo.InvariantCulture,
                CacheKey.JobsStop,
                InstanceName),
                cancellationToken);

            _logger.LogInformation("Job system starting every {JobSleepSeconds} seconds in instance {InstanceName}",
                JobSleepSeconds,
                InstanceName);

            if (!await GetCancellationOrder())
            {
                await Task.Delay(JobSleepSeconds * 1000, cancellationToken);

                while (!cancellationToken.IsCancellationRequested
                    && !await GetCancellationOrder())
                {
                    using (var scope = _services.CreateScope())
                    {
                        await scope.ServiceProvider
                                .GetRequiredService<JobTaskRunner>()
                                .ExecuteAsync(cancellationToken);
                    }

                    if (await GetCancellationOrder())
                    {
                        break;
                    }
                    else
                    {
                        await Task.Delay(JobSleepSeconds * 1000, cancellationToken);
                    }
                }
            }

            _logger.LogInformation("Job system shutting down in instance {InstanceName}",
                InstanceName);
        }

        private async Task<bool> GetCancellationOrder()
        {
            var cancellation = await _cache
                .GetStringAsync(string.Format(CultureInfo.InvariantCulture,
                    CacheKey.JobsStop,
                    InstanceName));

            if (cancellation == InstanceName)
            {
                await _cache.RemoveAsync(string.Format(CultureInfo.InvariantCulture,
                    CacheKey.JobsStop,
                    InstanceName));
                _logger.LogInformation("Job system shutdown request received for {InstanceName}",
                    InstanceName);
                return true;
            }

            return false;
        }
    }
}

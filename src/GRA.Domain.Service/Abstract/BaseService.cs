using System;
using System.Diagnostics;
using GRA.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service.Abstract
{
    public abstract class BaseService<Service>
    {
        protected readonly ILogger<Service> _logger;
        protected readonly IDateTimeProvider _dateTimeProvider;

        protected BaseService(ILogger<Service> logger,
            IDateTimeProvider dateTimeProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dateTimeProvider = dateTimeProvider
                ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        protected double GetElapsed(double start)
        {
            return (Stopwatch.GetTimestamp() - start) * 1000 / (double)Stopwatch.Frequency;
        }

        protected int GetPercent(int count, int total)
        {
            return count * 100 / total;
        }
    }
}

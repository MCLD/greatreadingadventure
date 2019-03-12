using System;
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
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;

namespace GRA.Domain.Service.Abstract
{
    public abstract class BaseService<Service>
    {
        protected readonly ILogger<Service> _logger;
        protected readonly IDateTimeProvider _dateTimeProvider;

        public BaseService(ILogger<Service> logger, IDateTimeProvider dateTimeProvider)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _dateTimeProvider = Require.IsNotNull(dateTimeProvider, nameof(dateTimeProvider));
        }
    }
}

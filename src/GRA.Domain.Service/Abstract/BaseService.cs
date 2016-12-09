using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Service.Abstract
{
    public abstract class BaseService<Service>
    {
        protected readonly ILogger<Service> _logger;

        public BaseService(ILogger<Service> logger)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
        }
    }
}

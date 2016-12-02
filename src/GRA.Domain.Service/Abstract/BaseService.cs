using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Service.Abstract
{
    public abstract class BaseService<T>
    {
        protected readonly ILogger<T> _logger;

        public BaseService(ILogger<T> logger)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
        }
    }
}

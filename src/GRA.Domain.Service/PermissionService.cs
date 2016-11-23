using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class PermissionService : Abstract.BaseService<PermissionService>
    {
        public PermissionService(ILogger<PermissionService> logger) : base(logger)
        {
        }
    }
}

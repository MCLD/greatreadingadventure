using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;

namespace GRA.Domain.Service.Abstract
{
    public abstract class BaseService<T>
    {
        protected readonly ILogger<T> logger;
        public BaseService(ILogger<T> logger)
        {
            if(logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            this.logger = logger;
        }

        protected int GetUserId(ClaimsIdentity user)
        {
            var userId = user.Claims.Where(_ => _.Type == Model.ClaimType.UserId).SingleOrDefault().Value;
            return int.Parse(userId);
        }
    }
}

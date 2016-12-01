using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GRA.Domain.Service;
using Microsoft.Extensions.Logging;
using GRA.Controllers.Base;

namespace GRA.Controllers.RouteConstraint
{
    public class SiteRouteConstraint : IRouteConstraint
    {
        private readonly ISitePathValidator _sitePathValidator;

        public SiteRouteConstraint(ISitePathValidator sitePathValidator)
        {
            _sitePathValidator = Require.IsNotNull(sitePathValidator, nameof(sitePathValidator));
        }

        public bool Match(HttpContext httpContext,
            IRouter route,
            string routeKey,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            string site = values[routeKey] as string;
            if (string.IsNullOrEmpty(site))
            {
                return false;
            }
            else
            {
                return _sitePathValidator.IsValid(site);
            }
        }
    }
}

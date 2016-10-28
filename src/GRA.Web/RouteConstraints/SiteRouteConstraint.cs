using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GRA.Web.RouteConstraints
{
    public class SiteRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext,
            IRouter route,
            string routeKey,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            var validSites = new[] { "site1", "site2" };

            if(values[routeKey] == null)
            {
                return false;
            }

            var site = values[routeKey].ToString();
            return validSites.Any(s => s == site.ToLower());
        }
    }
}

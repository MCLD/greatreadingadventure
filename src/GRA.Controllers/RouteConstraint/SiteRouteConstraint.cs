using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GRA.Controllers.RouteConstraint
{
    public class SiteRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext,
            IRouter route,
            string routeKey,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            var siteLookupService = httpContext
                .RequestServices
                .GetService(typeof(SiteLookupService)) as SiteLookupService;

            string sitePath = values[routeKey] as string;
            if (string.IsNullOrEmpty(sitePath))
            {
                return false;
            }
            else
            {
                var validSitePathLookup = siteLookupService.GetSitePathsAsync();
                Task.WaitAll(validSitePathLookup);
                return validSitePathLookup.Result.Any(_ => _ == sitePath.ToLower());
            }
        }
    }
}

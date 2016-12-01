using GRA.Domain.Service;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.Validator
{
    public class SitePathValidator : Base.ISitePathValidator
    {
        private readonly SiteService _siteService;

        public SitePathValidator(SiteService siteService)
        {
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
        }
        public bool IsValid(string sitePath)
        {
            var validSitePathLookup = _siteService.GetSitePaths();
            Task.WaitAll(validSitePathLookup);
            return validSitePathLookup.Result.Any(s => s == sitePath.ToLower());
        }
    }
}

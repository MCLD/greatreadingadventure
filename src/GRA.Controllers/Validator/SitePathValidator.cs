using GRA.Domain.Service;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.Validator
{
    public class SitePathValidator : Base.ISitePathValidator
    {
        private readonly SiteLookupService _siteLookupService;

        public SitePathValidator(SiteLookupService siteLookupService)
        {
            _siteLookupService = Require.IsNotNull(siteLookupService, nameof(siteLookupService));
        }
        public bool IsValid(string sitePath)
        {
            var validSitePathLookup = _siteLookupService.GetSitePaths();
            Task.WaitAll(validSitePathLookup);
            return validSitePathLookup.Result.Any(s => s == sitePath.ToLower());
        }
    }
}

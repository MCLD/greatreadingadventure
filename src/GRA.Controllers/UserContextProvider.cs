using GRA.Domain.Service.Abstract;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using GRA.Domain.Model;
using System.Threading.Tasks;
using System.Security.Claims;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace GRA.Controllers
{
    public class UserContextProvider : IUserContextProvider
    {
        private readonly ILogger<UserContextProvider> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SiteLookupService _siteLookupService;

        public UserContextProvider(ILogger<UserContextProvider> logger,
            IHttpContextAccessor httpContextAccessor,
            SiteLookupService siteLookupService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
        }

        public UserContext GetContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userContext = new UserContext
            {
                User = httpContext.User,
                SiteId = (int)httpContext.Items[ItemKey.SiteId],
                SiteStage = (SiteStage)httpContext.Items[ItemKey.SiteStage]
            };

            if (httpContext.User.Identity.IsAuthenticated)
            {
                userContext.ActiveUserId = httpContext.Session.GetInt32(SessionKey.ActiveUserId)
                    ?? GetId(httpContext.User, ClaimType.UserId);
            }
            return userContext;
        }

        public async Task<Site> GetCurrentSiteAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return await _siteLookupService.GetByIdAsync((int)httpContext.Items[ItemKey.SiteId]);
        }

        public bool UserHasPermission(ClaimsPrincipal user, string permissionName)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.HasClaim(ClaimType.Permission, permissionName);
        }

        public string UserClaim(ClaimsPrincipal user, string claimType)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var claim = user
                .Claims
                .Where(_ => _.Type == claimType);

            switch (claim.Count())
            {
                case 0:
                    return null;
                case 1:
                    return claim.First().Value;
                default:
                    string userId = user.Claims
                        .Where(_ => _.Type == ClaimType.UserId)
                        .FirstOrDefault()?.Value ?? "Unknown";
                    var distinct = claim.Distinct();
                    if (distinct.Count() > 1)
                    {
                        throw new Exception(string.Format("User {0} has multiple {1} claims: {2}",
                            userId,
                            claimType,
                            string.Join(",", claim.Select(_ => _.Value))));
                    }
                    else
                    {
                        _logger.LogWarning($"User {userId} has multiple {claimType} claims with the same value: {distinct.First().Value}");
                        return distinct.First().Value;
                    }
            }
        }


        public int GetId(ClaimsPrincipal user, string claimType)
        {
            string result = UserClaim(user, claimType);
            if (string.IsNullOrEmpty(result))
            {
                throw new Exception($"Could not find user claim '{claimType}'");
            }
            if (int.TryParse(result, out int id))
            {
                return id;
            }
            else
            {
                throw new Exception($"Could not convert '{claimType}' to a number.");
            }
        }

    }
}

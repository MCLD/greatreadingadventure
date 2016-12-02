using GRA.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GRA.Domain.Service.Abstract
{
    public abstract class BaseUserService<T> : BaseService<T>
    {
        protected readonly IUserContextProvider _userContextProvider;
        public BaseUserService(ILogger<T> logger, IUserContextProvider userContextProvider)
            : base(logger)
        {
            _userContextProvider = Require.IsNotNull(userContextProvider, nameof(userContextProvider));
        }

        private UserContext userContext = null;
        private ClaimsPrincipal currentUser = null;
        private int? currentUserSiteId = null;
        protected async Task<ClaimsPrincipal> GetCurrentUser()
        {
            if (userContext == null)
            {
                userContext = await _userContextProvider.GetContext();
            }
            if (currentUser == null)
            {
                currentUser = userContext.User;
            }
            return currentUser;
        }

        protected async Task<int> GetCurrentSiteId()
        {
            if (userContext == null)
            {
                userContext = await _userContextProvider.GetContext();
            }
            if (currentUserSiteId == null)
            {
                currentUserSiteId = userContext.SiteId;
            }
            return (int)currentUserSiteId;
        }

        protected async Task<bool> HasPermission(Permission permission)
        {
            var currentUser = await GetCurrentUser();
            return new UserClaimLookup(currentUser).UserHasPermission(permission.ToString());
        }

        protected async Task<int> GetClaimId(string claimType)
        {
            var currentUser = await GetCurrentUser();
            return new UserClaimLookup(currentUser).GetId(claimType);
        }
    }
}

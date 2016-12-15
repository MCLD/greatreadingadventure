using GRA.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GRA.Domain.Service.Abstract
{
    public abstract class BaseUserService<Service> : BaseService<Service>
    {
        protected readonly IUserContextProvider _userContextProvider;
        public BaseUserService(ILogger<Service> logger, IUserContextProvider userContextProvider)
            : base(logger)
        {
            _userContextProvider = Require.IsNotNull(userContextProvider, nameof(userContextProvider));
        }

        private UserContext _userContext = null;
        private ClaimsPrincipal _currentUser = null;
        private int? _currentUserSiteId = null;

        protected UserContext GetUserContext()
        {
            if (_userContext == null)
            {
                _userContext = _userContextProvider.GetContext();
            }
            return _userContext;
        }

        protected ClaimsPrincipal GetAuthUser()
        {
            if (_currentUser == null)
            {
                var userContext = GetUserContext();
                _currentUser = userContext.User;
            }
            return _currentUser;
        }

        protected int GetCurrentSiteId()
        {
            if (_currentUserSiteId == null)
            {
                var userContext = GetUserContext();
                _currentUserSiteId = userContext.SiteId;
            }
            return (int)_currentUserSiteId;
        }

        protected bool HasPermission(Permission permission)
        {
            var currentUser = GetAuthUser();
            return new UserClaimLookup(currentUser).UserHasPermission(permission.ToString());
        }

        protected int GetClaimId(string claimType)
        {
            var currentUser = GetAuthUser();
            return new UserClaimLookup(currentUser).GetId(claimType);
        }

        protected int GetActiveUserId()
        {
            var userContext = GetUserContext();
            if(userContext == null
               || !userContext.User.Identity.IsAuthenticated
               || userContext.ActiveUserId == null)
            {
                throw new System.Exception("User is not authenticated.");
            }
            return (int)userContext.ActiveUserId;
        }

        protected void VerifyCanRegister()
        {
            var userContext = GetUserContext();
            if (userContext.SiteStage != SiteStage.RegistrationOpen
                && userContext.SiteStage != SiteStage.ProgramOpen)
            {
                throw new GraException("The program is not accepting registrations at this time.");
            }
        }
    }
}

using GRA.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace GRA.Domain.Service.Abstract
{
    public abstract class BaseUserService<Service> : BaseService<Service>
    {
        protected readonly IUserContextProvider _userContextProvider;
        public BaseUserService(ILogger<Service> logger, 
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider)
            : base(logger, dateTimeProvider)
        {
            _userContextProvider = Require.IsNotNull(userContextProvider, nameof(userContextProvider));
        }

        private UserContext _userContext = null;
        private ClaimsPrincipal _currentUser = null;
        private int? _currentUserSiteId = null;
        private Permission? _managementPermission = null;

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
            return _userContextProvider.UserHasPermission(currentUser, permission.ToString());
            //return new UserClaimLookup(currentUser).UserHasPermission(permission.ToString());
        }

        protected int GetClaimId(string claimType)
        {
            var currentUser = GetAuthUser();
            return _userContextProvider.GetId(currentUser, claimType);
        }

        protected int GetActiveUserId()
        {
            var userContext = GetUserContext();
            if (userContext == null
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

        protected void VerifyCanLog()
        {
            var userContext = GetUserContext();
            if (userContext.SiteStage != SiteStage.ProgramOpen)
            {
                throw new GraException("The program is not open for activity at this time.");
            }
        }

        protected void VerifyCanHouseholdAction()
        {
            var userContext = GetUserContext();
            if (userContext.SiteStage != SiteStage.ProgramOpen && userContext.SiteStage != SiteStage.RegistrationOpen)
            {
                throw new GraException("Household changes cannot be made at this time.");
            }
        }

        protected void VerifyPermission(Permission permission)
        {
            if (!HasPermission(permission))
            {
                _logger.LogError($"User id {GetClaimId(ClaimType.UserId)} does not have permission {permission}.");
                throw new GraException("Permission denied.");
            }
        }

        protected void SetManagementPermission(Permission permission)
        {
            _managementPermission = permission;
        }

        protected void VerifyManagementPermission()
        {
            if (_managementPermission == null)
            {
                _logger.LogError("VerifyManagementPermission called before SetManagementPermission.");
                throw new System.Exception("An error occurred verifying permissions.");
            }
            var permission = (Permission)_managementPermission;
            if (!HasPermission(permission))
            {
                _logger.LogError($"User id {GetClaimId(ClaimType.UserId)} does not have permission {permission}.");
                throw new GraException("Permission denied.");
            }
        }

        public void ClearCachedUserContext()
        {
            _userContext = null;
        }
    }
}

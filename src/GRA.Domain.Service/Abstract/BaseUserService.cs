using System;
using System.Security.Claims;
using GRA.Domain.Model;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service.Abstract
{
    public abstract class BaseUserService<Service> : BaseService<Service>
    {
        protected const int DefaultCacheExpiration = 5;
        protected readonly IUserContextProvider _userContextProvider;
        private ClaimsPrincipal _currentUser = null;

        private int? _currentUserSiteId = null;

        private Permission? _managementPermission = null;

        private UserContext _userContext = null;

        protected BaseUserService(ILogger<Service> logger,
                                            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider)
            : base(logger, dateTimeProvider)
        {
            _userContextProvider = userContextProvider
                ?? throw new ArgumentNullException(nameof(userContextProvider));
        }

        public void ClearCachedUserContext()
        {
            _userContext = null;
        }

        public void VerifyCanHouseholdAction()
        {
            var userContext = GetUserContext();
            if (userContext.SiteStage != SiteStage.ProgramOpen
                && userContext.SiteStage != SiteStage.RegistrationOpen)
            {
                throw new GraException(Annotations.Validate.NotOpenActivity);
            }
        }

        protected DistributedCacheEntryOptions ExpireIn(int? minutes = null)
        {
            var fromMinutes = new TimeSpan(0, minutes ?? DefaultCacheExpiration, 0);
            return new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(fromMinutes);
        }

        protected TimeSpan ExpireInTimeSpan(int? minutes = null)
        {
            return new TimeSpan(0, minutes ?? DefaultCacheExpiration, 0);
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

        protected ClaimsPrincipal GetAuthUser()
        {
            if (_currentUser == null)
            {
                var userContext = GetUserContext();
                _currentUser = userContext.User;
            }
            return _currentUser;
        }

        protected int GetClaimId(string claimType)
        {
            var currentUser = GetAuthUser();
            return _userContextProvider.GetId(currentUser, claimType);
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

        protected UserContext GetUserContext()
        {
            if (_userContext == null)
            {
                _userContext = _userContextProvider.GetContext();
            }
            return _userContext;
        }

        protected bool HasPermission(Permission permission)
        {
            var currentUser = GetAuthUser();
            return _userContextProvider.UserHasPermission(currentUser, permission.ToString());
        }

        protected void SetManagementPermission(Permission permission)
        {
            _managementPermission = permission;
        }

        protected void VerifyCanLog()
        {
            var userContext = GetUserContext();
            if (userContext.SiteStage != SiteStage.ProgramOpen)
            {
                throw new GraException(Annotations.Validate.NotOpenActivity);
            }
        }

        protected void VerifyCanRegister()
        {
            var userContext = GetUserContext();
            if (userContext.SiteStage != SiteStage.RegistrationOpen
                && userContext.SiteStage != SiteStage.ProgramOpen)
            {
                throw new GraException(Annotations.Validate.NotOpen);
            }
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
                throw new GraException(Annotations.Validate.Permission);
            }
        }

        protected void VerifyPermission(Permission permission)
        {
            if (!HasPermission(permission))
            {
                _logger.LogError($"User id {GetClaimId(ClaimType.UserId)} does not have permission {permission}.");
                throw new GraException(Annotations.Validate.Permission);
            }
        }
    }
}

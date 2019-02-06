using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Controllers.Filter;
using GRA.Domain.Model;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace GRA.Controllers.Base
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [ServiceFilter(typeof(SiteFilter), Order = 1)]
    [SessionTimeoutFilter]
    public abstract class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        protected const string DropDownTrueValue = "True";
        protected const string DropDownFalseValue = "False";

        protected static readonly string[] ValidImageExtensions = { ".jpeg", ".jpg", ".png" };

        protected readonly IConfiguration _config;
        protected readonly IDateTimeProvider _dateTimeProvider;
        protected readonly IPathResolver _pathResolver;
        protected readonly IUserContextProvider _userContextProvider;
        protected readonly IStringLocalizer<Resources.Shared> _sharedLocalizer;
        protected readonly SiteLookupService _siteLookupService;
        protected string PageTitle { get; set; }
        protected string PageTitleHtml { get; set; }

        protected Controller(ServiceFacade.Controller context)
        {
            _config = context.Config;
            _dateTimeProvider = context.DateTimeProvider;
            _pathResolver = context.PathResolver;
            _userContextProvider = context.UserContextProvider;
            _sharedLocalizer = context.SharedLocalizer;
            _siteLookupService = context.SiteLookupService;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            // sensible default
            string pageTitle = _config[ConfigurationKey.DefaultSiteName];

            // set page title
            if (context.Controller is Controller controller
                && !string.IsNullOrWhiteSpace(controller.PageTitle))
            {
                pageTitle = controller.PageTitle;
            }
            ViewData[ViewDataKey.Title] = pageTitle;
            ViewData[ViewDataKey.TitleHtml] = PageTitleHtml;
        }

        protected string AlertDanger
        {
            set
            {
                TempData[TempDataKey.AlertDanger] = value;
            }
        }

        protected string AlertWarning
        {
            set
            {
                TempData[TempDataKey.AlertWarning] = value;
            }
        }

        protected string AlertInfo
        {
            set
            {
                TempData[TempDataKey.AlertInfo] = value;
            }
        }

        protected string AlertSuccess
        {
            set
            {
                TempData[TempDataKey.AlertSuccess] = value;
            }
        }

        public IActionResult Error()
        {
            return View();
        }

        protected async Task LoginUserAsync(AuthenticationResult authResult)
        {
            if (authResult.FoundUser && authResult.PasswordIsValid)
            {
                var claims = new HashSet<Claim>();
                foreach (var permissionName in authResult.PermissionNames)
                {
                    claims.Add(new Claim(ClaimType.Permission, permissionName));
                }
                claims.Add(new Claim(ClaimType.BranchId, authResult.User.BranchId.ToString()));
                claims.Add(new Claim(ClaimType.SiteId, authResult.User.SiteId.ToString()));
                claims.Add(new Claim(ClaimType.SystemId, authResult.User.SystemId.ToString()));
                claims.Add(new Claim(ClaimType.UserId, authResult.User.Id.ToString()));
                claims.Add(new Claim(ClaimType.AuthenticatedAt, _dateTimeProvider
                    .Now
                    .ToString("O")));

                var identity = new ClaimsIdentity(claims, Authentication.TypeGRAPassword);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));

                HttpContext.Session.SetInt32(SessionKey.ActiveUserId, authResult.User.Id);

                if (!string.IsNullOrEmpty(authResult.AuthenticationMessage))
                {
                    AlertInfo = authResult.AuthenticationMessage;
                }
            }
            else
            {
                ShowAlertDanger(authResult.AuthenticationMessage);
            }
        }

        public async Task LogoutUserAsync()
        {
            int? siteId = HttpContext.Session.GetInt32(SessionKey.SiteId);
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.User = null;
            if (siteId != null)
            {
                HttpContext.Session.SetInt32(SessionKey.SiteId, (int)siteId);
            }
        }

        protected ClaimsPrincipal AuthUser
        {
            get
            {
                return HttpContext.User;
            }
        }

        protected bool UserHasPermission(Permission permission)
        {
            return _userContextProvider.UserHasPermission(AuthUser, permission.ToString());
        }

        protected string UserClaim(string claimType)
        {
            return _userContextProvider.UserClaim(AuthUser, claimType);
        }

        protected int GetId(string claimType)
        {
            return _userContextProvider.GetId(AuthUser, claimType);
        }

        protected int GetCurrentSiteId()
        {
            return _userContextProvider.GetContext().SiteId;
        }

        protected async Task<Site> GetCurrentSiteAsync()
        {
            return await _siteLookupService.GetByIdAsync(GetCurrentSiteId());
        }

        protected int GetActiveUserId()
        {
            int? activeUserId = HttpContext.Session.GetInt32(SessionKey.ActiveUserId)
                ?? (int?)GetId(ClaimType.UserId);
            return (int)activeUserId;
        }

        protected SiteStage GetSiteStage()
        {
            return (SiteStage)HttpContext.Items[ItemKey.SiteStage];
        }

        protected string FormatMessage(GraException gex)
        {
            if (gex.Message.Contains(Environment.NewLine))
            {
                var lines = gex.Message.Split(
                    new string[] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries);
                var formatted = new StringBuilder("<ul>");
                foreach (string line in lines)
                {
                    formatted.Append("<li>").Append(line).Append("</li>");
                }
                formatted.Append("</ul>");
                return formatted.ToString();
            }
            return gex.Message;
        }

        private string Fa(string iconName)
        {
            return $"<span class=\"fa fa-{iconName}\" aria-hidden=\"true\"></span>";
        }

        protected void ShowAlertDanger(string message, string details = null)
        {
            AlertDanger = $"{Fa("exclamation-triangle")} {message}{details}";
        }

        protected void ShowAlertDanger(string message, GraException gex)
        {
            AlertDanger = $"{Fa("exclamation-triangle")} {message}{FormatMessage(gex)}";
        }

        protected void ShowAlertWarning(string message, string details = null)
        {
            AlertWarning = $"{Fa("exclamation-circle")} {message}{details}";
        }

        protected void ShowAlertWarning(string message, GraException gex)
        {
            AlertWarning = $"{Fa("exclamation-circle")} {message}{FormatMessage(gex)}";
        }

        protected void ShowAlertSuccess(string message, string faIconName = null)
        {
            AlertSuccess = string.IsNullOrEmpty(faIconName)
                ? $"{Fa("thumbs-o-up")} {message}"
                : $"{Fa(faIconName)} {message}";
        }

        protected void ShowAlertInfo(string message, string faIconName = null)
        {
            AlertInfo = string.IsNullOrEmpty(faIconName)
                ? $"{Fa("check-circle")} {message}"
                : $"{Fa(faIconName)} {message}";
        }

        /// <summary>
        /// Look up if a site setting is set by key.
        /// </summary>
        /// <param name="key">The site setting key value (a string, up to 255 characters)</param>
        /// <returns>True if the value is set in the database, false if the key is not present or
        /// set to NULL.</returns>
        protected async Task<bool> IsSiteSettingSetAsync(string key)
        {
            return await _siteLookupService.IsSiteSettingSetAsync(GetCurrentSiteId(), key);
        }

        /// <summary>
        /// Look up a boolean site setting by key.
        /// </summary>
        /// <param name="key">The site setting key value (a string, up to 255 characters)</param>
        /// <returns>True if the value is set in the database, false if the key is not present or
        /// set to NULL.</returns>
        protected async Task<bool> GetSiteSettingBoolAsync(string key)
        {
            return await _siteLookupService.GetSiteSettingBoolAsync(GetCurrentSiteId(), key);
        }

        /// <summary>
        /// Look up an integer site setting by key.
        /// </summary>
        /// <param name="key">The site setting key value (a string, up to 255 characters)</param>
        /// <returns>A tuple, the bool is true if the setting is present and a number with the
        /// value being the number. The bool is false if the setting is not set or is not a
        /// parsable integer.</returns>
        protected async Task<(bool, int)> GetSiteSettingIntAsync(string key)
        {
            return await _siteLookupService.GetSiteSettingIntAsync(GetCurrentSiteId(), key);
        }

        /// <summary>
        /// Look up an string site setting by key.
        /// </summary>
        /// <param name="key">The site setting key value (a string, up to 255 characters)</param>
        /// <returns>A tuple, the bool is true if the setting is present and a string with the
        /// value . The bool is false if the setting is not set.</returns>
        /// integer.</returns>
        protected async Task<(bool, string)> GetSiteSettingStringAsync(string key)
        {
            return await _siteLookupService.GetSiteSettingStringAsync(GetCurrentSiteId(), key);
        }

        /// <summary>
        /// Construct a drop-down list with a blank (default) option along with No and Yes options.
        /// </summary>
        /// <returns>A SelectList with empty, No, and Yes options. Keys are 
        /// <see cref="DropDownFalseValue"/> for no and <see cref="DropDownTrueValue"/> for yes.
        /// </returns>
        protected SelectList EmptyNoYes()
        {
            return new SelectList(new Dictionary<string, string>
            {
                {string.Empty, string.Empty},
                {DropDownFalseValue, "No" },
                {DropDownTrueValue, "Yes" }
            },
            "Key",
            "Value",
            string.Empty);
        }
    }
}

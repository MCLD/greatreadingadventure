using System;
using System.Reflection;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.Filter
{
    public class UserFilter : Attribute, IAsyncActionFilter
    {
        private readonly ILogger _logger;
        private readonly IStringLocalizer<Resources.Shared> _sharedLocalizer;
        private readonly ITempDataDictionaryFactory _tempDataFactory;
        private readonly MailService _mailService;
        private readonly PerformerSchedulingService _performerSchedulingService;
        private readonly SiteLookupService _siteLookupService;
        private readonly UserService _userService;
        private readonly IUserContextProvider _userContextProvider;

        public UserFilter(ILogger<UserFilter> logger,
            IStringLocalizer<Resources.Shared> sharedLocalizer,
            ITempDataDictionaryFactory tempDataFactory,
            MailService mailService,
            PerformerSchedulingService performerSchedulingService,
            SiteLookupService siteLookupService,
            UserService userService,
            IUserContextProvider userContextProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tempDataFactory = tempDataFactory
                ?? throw new ArgumentNullException(nameof(tempDataFactory));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _performerSchedulingService = performerSchedulingService
                ?? throw new ArgumentNullException(nameof(performerSchedulingService));
            _sharedLocalizer = sharedLocalizer
                ?? throw new ArgumentNullException(nameof(sharedLocalizer));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userContextProvider = userContextProvider
                ?? throw new ArgumentNullException(nameof(userContextProvider));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                // Check if the user Id matches the active user id
                if (httpContext.Session.GetInt32(SessionKey.ActiveUserId)
                        == _userContextProvider.GetId(httpContext.User, ClaimType.UserId))
                {
                    // Check if user can access mission control
                    if (httpContext.User.HasClaim(ClaimType.Permission,
                        nameof(Permission.AccessMissionControl)))
                    {
                        httpContext.Items.Add(ItemKey.ShowMissionControl, true);
                    }

                    // Check if user can access performer registration
                    if (httpContext.User.HasClaim(ClaimType.Permission,
                        nameof(Permission.AccessPerformerRegistration)))
                    {
                        var settings = await _performerSchedulingService.GetSettingsAsync();
                        var schedulingStage = _performerSchedulingService
                            .GetSchedulingStage(settings);
                        if (schedulingStage != PsSchedulingStage.Unavailable)
                        {
                            httpContext.Items.Add(ItemKey.ShowPerformerRegistration, true);
                        }
                    }
                }

                var pendingQuestionnaire = httpContext
                    .Session
                    .GetInt32(SessionKey.PendingQuestionnaire);

                if (pendingQuestionnaire.HasValue)
                {
                    var controllerActionDescriptor
                        = context.ActionDescriptor as ControllerActionDescriptor;
                    if (!controllerActionDescriptor.ControllerTypeInfo
                            .IsDefined(typeof(Attributes.PreventQuestionnaireRedirect))
                        && !controllerActionDescriptor.MethodInfo
                            .IsDefined(typeof(Attributes.PreventQuestionnaireRedirect)))
                    {
                        var controller = (Base.Controller)context.Controller;
                        context.Result = controller.RedirectToAction("Index",
                            "Questionnaire",
                            new { id = pendingQuestionnaire });
                        return;
                    }
                }
                try
                {
                    httpContext.Items[ItemKey.UnreadCount]
                        = await _mailService.GetUserUnreadCountAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting user unread mail count: {Message}", ex.Message);
                }

                var tempData = _tempDataFactory.GetTempData(httpContext);

                if (tempData.ContainsKey(TempDataKey.UserSignedIn))
                {
                    tempData.Remove(TempDataKey.UserSignedIn);
                    httpContext.Items[ItemKey.SignedIn] = true;
                }

                var siteId = httpContext.Session.GetInt32(SessionKey.SiteId);
                int? activeUserId = _userContextProvider.GetContext().ActiveUserId;
                if (await _siteLookupService.GetSiteSettingBoolAsync((int)siteId,
                    SiteSettingKey.Users.ShowLinkToParticipantsLibrary)
                    && activeUserId != null)
                {
                    var userBranch = await _userService.GetUsersBranch(activeUserId.Value);
                    httpContext.Session.SetString(SessionKey.UserBranchName,
                        _sharedLocalizer[Annotations.Interface.Visit, userBranch.Name]);
                    httpContext.Session.SetString(SessionKey.UserBranchUrl, userBranch.Url);
                }
            }
            await next();
        }
    }
}

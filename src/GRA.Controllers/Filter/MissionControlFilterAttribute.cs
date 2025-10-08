using System;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.Filter
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MissionControlFilterAttribute : Attribute, IAsyncResourceFilter
    {
        public MissionControlFilterAttribute(ILogger<MissionControlFilterAttribute> logger,
            IUserContextProvider userContextProvider,
            MailService mailService,
            PerformerSchedulingService performerSchedulingService,
            QuestionnaireService questionnaireService,
            SiteLookupService siteLookupService,
            UserService userService)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(mailService);
            ArgumentNullException.ThrowIfNull(performerSchedulingService);
            ArgumentNullException.ThrowIfNull(questionnaireService);
            ArgumentNullException.ThrowIfNull(siteLookupService);
            ArgumentNullException.ThrowIfNull(userContextProvider);
            ArgumentNullException.ThrowIfNull(userService);

            Logger = logger;
            MailService = mailService;
            PerformerSchedulingService = performerSchedulingService;
            QuestionnaireService = questionnaireService;
            SiteLookupService = siteLookupService;
            UserContextProvider = userContextProvider;
            UserService = userService;
        }

        public ILogger<MissionControlFilterAttribute> Logger { get; }
        public MailService MailService { get; }
        public PerformerSchedulingService PerformerSchedulingService { get; }
        public QuestionnaireService QuestionnaireService { get; }
        public SiteLookupService SiteLookupService { get; }
        public IUserContextProvider UserContextProvider { get; }
        public UserService UserService { get; }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context,
            ResourceExecutionDelegate next)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(next);

            var httpContext = context.HttpContext;
            try
            {
                var userId = UserContextProvider.GetId(httpContext.User, ClaimType.UserId);
                var activeId = httpContext.Session.GetInt32(SessionKey.ActiveUserId);
                if (userId != activeId)
                {
                    httpContext.Session.SetInt32(SessionKey.ActiveUserId, userId);
                    var user = await UserService.GetDetails(userId);
                    var questionnaireId = await QuestionnaireService
                            .GetRequiredQuestionnaire(user.Id, user.Age);
                    if (questionnaireId.HasValue)
                    {
                        httpContext.Session.SetInt32(SessionKey.PendingQuestionnaire,
                            questionnaireId.Value);
                    }
                    else
                    {
                        httpContext.Session.Remove(SessionKey.PendingQuestionnaire);
                    }
                }
            }
            catch (GraException gex)
            {
                Logger.LogTrace(gex,
                    "Attempted Mission Control access while not logged in: {Message}",
                    gex.Message);
            }

            if (httpContext.User.HasClaim(ClaimType.Permission, nameof(Permission.ReadAllMail)))
            {
                try
                {
                    httpContext.Items[ItemKey.UnreadCount]
                        = await MailService.GetAdminUnreadCountAsync();
                }
                catch (GraException gex)
                {
                    Logger.LogError(gex,
                        "Error getting admin mail unread count: {Message}",
                        gex.Message);
                }
            }

            if (httpContext.User.HasClaim(ClaimType.Permission,
                nameof(Permission.ViewPerformerDetails)))
            {
                var settings = await PerformerSchedulingService.GetSettingsAsync();
                var schedulingStage = PerformerSchedulingService
                    .GetSchedulingStage(settings);
                if (schedulingStage != PsSchedulingStage.Unavailable)
                {
                    httpContext.Items.Add(ItemKey.ShowPerformerScheduling, true);
                }
            }

            var siteId = httpContext.Session.GetInt32(SessionKey.SiteId);
            if (siteId != null && await SiteLookupService.GetSiteSettingBoolAsync((int)siteId,
                SiteSettingKey.VendorCodes.ShowPackingSlip))
            {
                httpContext.Items.Add(ItemKey.ShowPackingSlips, true);
            }

            await next();
        }
    }
}

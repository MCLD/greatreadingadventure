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
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.Filter
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class UserFilterAttribute(
        ILogger<UserFilterAttribute> logger,
        ITempDataDictionaryFactory tempDataFactory,
        MailService mailService,
        PerformerSchedulingService performerSchedulingService,
        UserService userService,
        IUserContextProvider userContextProvider) : Attribute, IAsyncActionFilter
    {
        public ILogger<UserFilterAttribute> Logger { get; } = logger;
        public MailService MailService { get; } = mailService;

        public PerformerSchedulingService PerformerSchedulingService { get; }
            = performerSchedulingService;

        public ITempDataDictionaryFactory TempDataFactory { get; } = tempDataFactory;
        public IUserContextProvider UserContextProvider { get; } = userContextProvider;
        public UserService UserService { get; } = userService;

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "Failure in fetching mail at this point should not crash the filter")]
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(next);

            var httpContext = context.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                // Check if the user Id matches the active user id
                if (httpContext.Session.GetInt32(SessionKey.ActiveUserId)
                        == UserContextProvider.GetId(httpContext.User, ClaimType.UserId))
                {
                    // Check if user can access mission control
                    if (httpContext.User.HasClaim(ClaimType.Permission,
                        nameof(Permission.AccessMissionControl)))
                    {
                        httpContext.Items[ItemKey.ShowMissionControl] = true;
                    }

                    // Check if user can access performer registration
                    if (httpContext.User.HasClaim(ClaimType.Permission,
                        nameof(Permission.AccessPerformerRegistration)))
                    {
                        var settings = await PerformerSchedulingService.GetSettingsAsync();
                        var schedulingStage = PerformerSchedulingService
                            .GetSchedulingStage(settings);
                        if (schedulingStage != PsSchedulingStage.Unavailable)
                        {
                            httpContext.Items[ItemKey.ShowPerformerRegistration] = true;
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
                        context.Result = controller.RedirectToAction(
                            nameof(QuestionnaireController.Index),
                            "Questionnaire",
                            new { id = pendingQuestionnaire });
                        return;
                    }
                }

                try
                {
                    httpContext.Items[ItemKey.UnreadCount]
                        = await MailService.GetUserUnreadCountAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        ex,
                        "Error getting user unread mail count: {Message}",
                        ex.Message);
                }

                var tempData = TempDataFactory.GetTempData(httpContext);

                if (tempData.ContainsKey(TempDataKey.UserSignedIn))
                {
                    httpContext.Items[ItemKey.SignedIn] = true;
                    tempData.Remove(TempDataKey.UserSignedIn);
                }
            }
            await next();
        }
    }
}

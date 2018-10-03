﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.Filter
{
    public class UserFilter : Attribute, IAsyncActionFilter
    {
        private readonly ILogger _logger;
        private readonly MailService _mailService;
        private readonly UserService _userService;
        private readonly IUserContextProvider _userContextProvider;

        public UserFilter(ILogger<UserFilter> logger,
            MailService mailService,
            UserService userService,
            IUserContextProvider userContextProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
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
                // Check if user can access mission control and the user Id matches the active user id
                if (httpContext.User.HasClaim(GRA.ClaimType.Permission,
                        GRA.Domain.Model.Permission.AccessMissionControl.ToString())
                    && httpContext.Session.GetInt32(SessionKey.ActiveUserId) ==
                        _userContextProvider.GetId(httpContext.User, ClaimType.UserId))
                {
                    httpContext.Items.Add(ItemKey.ShowMissionControl, true);
                }

                var pendingQuestionnaire = httpContext.Session.GetInt32(SessionKey.PendingQuestionnaire);
                if (pendingQuestionnaire.HasValue)
                {
                    var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                    if (!controllerActionDescriptor.ControllerTypeInfo
                            .IsDefined(typeof(Attributes.PreventQuestionnaireRedirect))
                        && !controllerActionDescriptor.MethodInfo
                            .IsDefined(typeof(Attributes.PreventQuestionnaireRedirect)))
                    {
                        var controller = (Base.Controller)context.Controller;
                        context.Result = controller.RedirectToAction("Index", "Questionnaire", new { id = pendingQuestionnaire });
                        return;
                    }
                }
                try
                {
                    httpContext.Items[ItemKey.UnreadCount] = await _mailService.GetUserUnreadCountAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting user unread mail count: {Message}", ex.Message);
                }
            }
            await next();
        }
    }
}

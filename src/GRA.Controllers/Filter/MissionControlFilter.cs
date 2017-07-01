using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using GRA.Domain.Service.Abstract;

namespace GRA.Controllers.Filter
{
    public class MissionControlFilter : Attribute, IAsyncResourceFilter
    {
        private readonly ILogger<MissionControlFilter> _logger;
        private readonly IUserContextProvider _userContextProvider;
        private readonly MailService _mailService;
        private readonly QuestionnaireService _questionnaireService;
        private readonly UserService _userService;

        public MissionControlFilter(ILogger<MissionControlFilter> logger,
            IUserContextProvider userContextProvider,
            MailService mailService,
            QuestionnaireService questionnaireService,
            UserService userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userContextProvider = userContextProvider 
                ?? throw new ArgumentNullException(nameof(userContextProvider));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _questionnaireService = questionnaireService 
                ?? throw new ArgumentNullException(nameof(questionnaireService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context,
            ResourceExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            try
            {
                var userId = _userContextProvider.GetId(httpContext.User, ClaimType.UserId);
                var activeId = httpContext.Session.GetInt32(SessionKey.ActiveUserId);
                if (userId != activeId)
                {
                    httpContext.Session.SetInt32(SessionKey.ActiveUserId, userId);
                    var user = await _userService.GetDetails(userId);
                    var questionnaireId = await _questionnaireService
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
            catch (Exception ex)
            {
                _logger.LogDebug($"Attempted Mission Control access while not logged in: {ex.Message}");
            }

            if (httpContext.User.HasClaim(ClaimType.Permission,
                Domain.Model.Permission.ReadAllMail.ToString()))
            {
                httpContext.Items[ItemKey.UnreadCount] = await _mailService.GetAdminUnreadCountAsync();
            }

            await next();
        }
    }
}

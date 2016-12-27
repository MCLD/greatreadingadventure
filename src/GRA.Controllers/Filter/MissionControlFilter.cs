using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GRA.Controllers.Filter
{
    public class MissionControlFilter : Attribute, IAsyncResourceFilter
    {
        private readonly ILogger<MissionControlFilter> _logger;
        private readonly MailService _mailService;

        public MissionControlFilter(ILogger<MissionControlFilter> logger, MailService mailService)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mailService = Require.IsNotNull(mailService, nameof(mailService));
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context,
            ResourceExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            try {
                var userId = new UserClaimLookup(httpContext.User).GetId(ClaimType.UserId);
                var activeId = httpContext.Session.GetInt32(SessionKey.ActiveUserId);
                if (userId != activeId)
                {
                    httpContext.Session.SetInt32(SessionKey.ActiveUserId, userId);
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

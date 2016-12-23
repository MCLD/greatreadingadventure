using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace GRA.Controllers.Filter
{
    public class UserFilter : Attribute, IAsyncResourceFilter
    {
        private readonly MailService _mailService;

        public UserFilter(MailService mailService)
        {
            _mailService = Require.IsNotNull(mailService, nameof(mailService));
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context,
            ResourceExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                httpContext.Items[ItemKey.UnreadCount] = await _mailService.GetUserUnreadCountAsync();
            }

            await next();
        }
    }
}

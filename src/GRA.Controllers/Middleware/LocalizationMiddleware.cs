using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace GRA.Controllers.Middleware
{
    public class LocalizationMiddleware
    {
        public void Configure(
            IApplicationBuilder app,
            IOptions<RequestLocalizationOptions> l10nOptions)
        {
            app.UseRequestLocalization(l10nOptions.Value);
        }
    }
}

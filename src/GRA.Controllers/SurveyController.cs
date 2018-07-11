using System;
using System.Threading.Tasks;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    public class SurveyController : Base.UserController
    {
        private readonly ILogger<SurveyController> _logger;
        private readonly UserService _userService;
        public SurveyController(ILogger<SurveyController> logger,
            ServiceFacade.Controller context,
            UserService userService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = Require.IsNotNull(userService, nameof(userService));
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var siteId = GetCurrentSiteId();
            var survey = await _siteLookupService.GetSiteSettingStringAsync(siteId,
                SiteSettingKey.Users.SurveyUrl);
            if (survey.IsSet)
            {
                var askFirstTime = await _siteLookupService.GetSiteSettingBoolAsync(siteId,
                SiteSettingKey.Users.AskIfFirstTime);
                if (askFirstTime)
                {
                    var firstTimeSurvery = await _siteLookupService.GetSiteSettingStringAsync(siteId,
                        SiteSettingKey.Users.FirstTimeSurveyUrl);
                    if (firstTimeSurvery.IsSet)
                    {
                        var user = await _userService.GetDetails(GetActiveUserId());
                        if (user.IsFirstTime)
                        {
                            return Redirect(firstTimeSurvery.SetValue);
                        }
                    }
                }

                return Redirect(survey.SetValue);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}

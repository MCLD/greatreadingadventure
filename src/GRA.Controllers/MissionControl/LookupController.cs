using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewParticipantList)]
    public class LookupController : Base.MCController
    {
        private readonly ILogger<LookupController> _logger;
        private readonly TriggerService _triggerService;

        public LookupController(ILogger<LookupController> logger,
             ServiceFacade.Controller context,
             TriggerService triggerService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _triggerService = Require.IsNotNull(triggerService, nameof(triggerService));
        }

        [HttpPost]
        public async Task<JsonResult> SecretCodeInUse(string secretCode)
        {
            return Json(await _triggerService.SecretCodeInUseAsync(secretCode));
        }
    }
}

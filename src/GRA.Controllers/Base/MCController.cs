﻿using GRA.Controllers.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers.Base
{
    [ServiceFilter(typeof(MissionControlFilter), Order = 2)]
    public abstract class MCController : Controller
    {
        protected static readonly string[] ValidCsvExtensions = { ".CSV" };

        protected static readonly string[] ValidExcelExtensions = { ".xls", ".xlsx" };

        protected static readonly string[] ValidUploadExtensions
            = { ".jpeg", ".jpg", ".pdf", ".png" };

        protected MCController(ServiceFacade.Controller context) : base(context)
        {
        }

        protected static string GetBadgeMakerUrl(string origin, string email)
        {
            return $"https://www.openbadges.me/designer/index.html?origin={origin}&email={email}";
        }

        protected IActionResult RedirectNotAuthorized(string reason)
        {
            ShowAlertWarning(reason);
            return RedirectToAction(nameof(MissionControl.HomeController.Index),
                MissionControl.HomeController.Name,
                new
                {
                    area = nameof(MissionControl)
                });
        }

        protected string UnsubBase()
        {
            return Url.Action(nameof(HomeController.Unsubscribe),
                HomeController.Name,
                new { Area = "" },
                HttpContext.Request.Scheme);
        }
    }
}

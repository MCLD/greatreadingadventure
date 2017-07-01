using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GRA.Controllers.ServiceFacade;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageVendorCodes)]
    public class VendorCodesController : Base.MCController
    {
        private readonly SiteService _siteService;
        private readonly VendorCodeService _vendorCodeService;
        public VendorCodesController(ServiceFacade.Controller context,
            SiteService siteService,
            VendorCodeService vendorCodeService)
            : base(context)
        {
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _vendorCodeService = vendorCodeService
                ?? throw new ArgumentNullException(nameof(vendorCodeService));
            PageTitle = "Vendor Codes";
        }

        [HttpGet]
        public async Task<IActionResult> ImportStatus()
        {
            var codeTypes = await _vendorCodeService.GetTypeAllAsync();
            var codeTypeSelectList = codeTypes.Select(_ => new SelectListItem
            {
                Value = _.Id.ToString(),
                Text = _.Description

            });
            return View(codeTypeSelectList);
        }

        [HttpPost]
        public async Task<IActionResult> ImportStatus(int vendorCodeId,
            Microsoft.AspNetCore.Http.IFormFile excelFile)
        {
            if (excelFile == null
                || Path.GetExtension(excelFile.FileName).ToLower() != ".xls")
            {
                AlertDanger = "You must select an .xls file.";
                ModelState.AddModelError("excelFile", "You must select an .xls file.");
            }

            if (ModelState.ErrorCount == 0)
            {
                var tempFile = Path.GetTempFileName();
                using (var fileStream = new FileStream(tempFile, FileMode.Create))
                {
                    await excelFile.CopyToAsync(fileStream);
                }
                string file = WebUtility.UrlEncode(Path.GetFileName(tempFile));
                return RedirectToAction("ImportFile", new { id = file });
            }
            else
            {
                return RedirectToAction("ImportStatus");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ImportFile(string id)
        {
            PageTitle = "Import Vendor Status";

            var wsUrl = await _siteService.GetWsUrl(Request.Scheme, Request.Host.Value);

            return View("ImportFile", $"{wsUrl}/MissionControl/processvendor/{id}");
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageVendorCodes)]
    public class VendorCodesController : Base.MCController
    {
        private readonly ILogger _logger;
        private readonly JobService _jobService;
        private readonly VendorCodeService _vendorCodeService;

        public VendorCodesController(ServiceFacade.Controller context,
            ILogger<VendorCodesController> logger,
            JobService jobService,
            VendorCodeService vendorCodeService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _vendorCodeService = vendorCodeService
                ?? throw new ArgumentNullException(nameof(vendorCodeService));
            PageTitle = "Vendor code management";
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
                || !string.Equals(Path.GetExtension(excelFile.FileName), ".xls",
                    StringComparison.OrdinalIgnoreCase))
            {
                AlertDanger = "You must select an .xls file.";
                ModelState.AddModelError("excelFile", "You must select an .xls file.");
                return RedirectToAction("ImportStatus");
            }

            if (ModelState.ErrorCount == 0)
            {
                var tempFile = _pathResolver.ResolvePrivateTempFilePath();
                _logger.LogInformation("Accepted vendor id {vendorCodeId} import file {UploadFile} as {TempFile}",
                    vendorCodeId,
                    excelFile.FileName,
                    tempFile);

                using (var fileStream = new FileStream(tempFile, FileMode.Create))
                {
                    await excelFile.CopyToAsync(fileStream);
                }

                string file = WebUtility.UrlEncode(Path.GetFileName(tempFile));

                var jobToken = await _jobService.CreateJobAsync(new Job
                {
                    JobType = JobType.UpdateVendorStatus,
                    SerializedParameters = JsonConvert
                        .SerializeObject(new JobDetailsVendorCodeStatus
                        {
                            Filename = file
                        })
                });

                return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
                {
                    CancelUrl = Url.Action(nameof(ImportStatus)),
                    JobToken = jobToken.ToString(),
                    PingSeconds = 5,
                    SuccessRedirectUrl = "",
                    SuccessUrl = Url.Action(nameof(ImportStatus)),
                    Title = "Loading import..."
                });
            }
            else
            {
                return RedirectToAction("ImportStatus");
            }
        }
    }
}

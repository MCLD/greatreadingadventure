using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.DailyTips;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageDailyLiteracyTips)]
    public class DailyTipsController : Base.MCController
    {
        private const long MaxFileSize = 100L * 1024L * 1024L;

        private readonly DailyLiteracyTipService _dailyLiteracyTipService;

        public DailyTipsController(ServiceFacade.Controller context,
            DailyLiteracyTipService dailyLiteracyTipService)
            : base(context)
        {
            _dailyLiteracyTipService = dailyLiteracyTipService
                ?? throw new ArgumentNullException(nameof(dailyLiteracyTipService));

            PageTitle = "Daily Tips";
        }

        public static string Name { get { return "DailyTips"; } }

        [HttpGet]
        public async Task<IActionResult> Index(int page)
        {
            page = page == 0 ? 1 : page;

            var filter = new BaseFilter(page);

            var tips = await _dailyLiteracyTipService.GetPaginatedListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = tips.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };

            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var tipCount = new System.Collections.Generic.Dictionary<int, int>();
            foreach (var tip in tips.Data)
            {
                tipCount.Add(tip.Id,
                    await _dailyLiteracyTipService.GetImagesByTipIdAsync(tip.Id));
            }

            return View(new TipListViewModel
            {
                DailyTips = tips.Data,
                PaginateModel = paginateModel,
                TipCount = tipCount
            });
        }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var site = await GetCurrentSiteAsync();

            if (site.ProgramStarts.HasValue && site.ProgramEnds.HasValue)
            {
                var programDays = (int)Math.Ceiling((site.ProgramEnds.Value - site.ProgramStarts.Value).TotalDays);
                var scheduleUrl = Url.Action("Schedule", "Sites", new { area = "MissionControl", id = 1 });
                if (programDays > 0)
                {
                    ShowAlertInfo($"You will need <strong>{programDays} </strong> daily image(s) to ensure you have one for each day of your program. " + $"Visit <a href = '{scheduleUrl}'>Site Schedule</a> to view or adjust.");
                }
            }
            return View();
        }

        [HttpPost]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        public async Task<IActionResult> Upload(TipUploadViewModel viewmodel)
        {
            if (viewmodel?.UploadedFile == null
                || !string.Equals(Path.GetExtension(viewmodel.UploadedFile.FileName), ".zip",
                    StringComparison.OrdinalIgnoreCase))
            {
                AlertDanger = "You must select a .zip file.";
                ModelState.AddModelError("UploadedFile", "You must select a .zip file.");
            }

            if (ModelState.IsValid && viewmodel != null)
            {
                var dailyTip = await _dailyLiteracyTipService.AddAsync(new DailyLiteracyTip
                {
                    IsLarge = viewmodel.IsLarge,
                    Message = viewmodel.Message,
                    Name = viewmodel.Name,
                });

                if (dailyTip == null)
                {
                    ShowAlertDanger("Unable to create Daily Tip in the database.");
                    return RedirectToAction(nameof(Upload));
                }

                try
                {
                    using var archive = new ZipArchive(viewmodel.UploadedFile.OpenReadStream());

                    var (added, issues)
                        = await _dailyLiteracyTipService.AddImagesZipAsync(dailyTip.Id, archive);

                    if (issues?.Count > 0)
                    {
                        var warning = new StringBuilder("<strong>Added ")
                            .Append(added)
                            .Append("</strong> daily images, encountered the following <strong>")
                            .Append(issues.Count)
                            .AppendLine(" issues</strong>:<ul>");
                        foreach (var issue in issues)
                        {
                            warning.Append("<li>").Append(issue).AppendLine("</li>");
                        }
                        ShowAlertWarning(warning.AppendLine("</ul>").ToString());
                    }
                    else
                    {
                        ShowAlertSuccess($"Added {added} daily images.");
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (GraException gex)
                {
                    ShowAlertDanger($"An error occurred adding images: {gex.Message}");
                    await _dailyLiteracyTipService.RemoveAsync(dailyTip.Id);
                }
            }

            return RedirectToAction(nameof(Upload));
        }
    }
}

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;
using GRA.Abstract;
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

        private readonly IPathResolver _pathResolver;

        public DailyTipsController(ServiceFacade.Controller context,
            DailyLiteracyTipService dailyLiteracyTipService,
            IPathResolver pathResolver)
            : base(context)
        {
            _dailyLiteracyTipService = dailyLiteracyTipService
                ?? throw new ArgumentNullException(nameof(dailyLiteracyTipService));

            _pathResolver = pathResolver
                ?? throw new ArgumentNullException(nameof(pathResolver));

            PageTitle = "Daily Tips";
        }

        public static string Name
        { get { return "DailyTips"; } }

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

            var site = await GetCurrentSiteAsync();

            if (site.ProgramStarts.HasValue && site.ProgramEnds.HasValue)
            {
                var days = (int)Math.Ceiling(
                    (site.ProgramEnds.Value - site.ProgramStarts.Value).TotalDays);
                if (days > 0)
                {
                    var programLink = Url.Action(nameof(ProgramsController.Index),
                        ProgramsController.Name,
                        new { area = ProgramsController.Area });

                    ShowAlertInfo($"You will need <strong>{days}</strong> daily image(s) to ensure you have one for each day of your program. Visit <a href=\"{programLink}\">Program Management</a> to assign daily tips to a program.");
                }
            }

            return View(new TipListViewModel
            {
                DailyTips = tips.Data,
                PaginateModel = paginateModel,
                TipCount = tipCount
            });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int tipId, int page)
        {
            page = page == 0 ? 1 : page;

            var filter = new DailyImageFilter(page)
            {
                DailyLiteracyTipId = tipId,
            };

            var imageData = await _dailyLiteracyTipService.GetPaginatedImageListAsync(filter);
            var tip = await _dailyLiteracyTipService.GetByIdAsync(tipId);
            var site = await GetCurrentSiteAsync();

            var imageModels = imageData.Data.Select(_ => new DailyLiteracyTipImageViewModel
            {
                Id = _.Id,
                Day = _.Day,
                Name = _.Name,
                Extension = _.Extension,
                ImagePath = _pathResolver.ResolveContentPath($"/site{site.Id}/dailyimages/dailyliteracytip{tip.Id}/{_.Name}{_.Extension}")
            }
            );

            var model = new TipDetailViewModel
            {
                Tip = tip,
                Images = imageModels.ToList(),
                ItemCount = imageData.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };

            if (model.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = model.LastPage ?? 1
                    });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var site = await GetCurrentSiteAsync();

            if (site.ProgramStarts.HasValue && site.ProgramEnds.HasValue)
            {
                var days = (int)Math.Ceiling(
                    (site.ProgramEnds.Value - site.ProgramStarts.Value).TotalDays);
                if (days > 0)
                {
                    var scheduleLink = Url.Action(nameof(SitesController.Schedule),
                        SitesController.Name,
                        new { area = SitesController.Area, id = 1 });

                    ShowAlertInfo($"You will need <strong>{days}</strong> daily image(s) to ensure you have one for each day of your program. Visit <a href=\"{scheduleLink}\">Site Schedule</a> to view or adjust your program schedule.");
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Add(int tipId)
        {
            var tip = await _dailyLiteracyTipService.GetByIdAsync(tipId);

            if (tip == null)
            {
                ShowAlertDanger($"Tip not found with ID <strong>{tipId}</strong>.");
                return RedirectToAction(nameof(Index));
            }

            return View(new TipImageAddViewModel
            {
                DailyTipId = tipId
            });
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

        [HttpPost]
        public async Task<IActionResult> Add(TipImageAddViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var tip = await _dailyLiteracyTipService.GetByIdAsync(viewModel.DailyTipId);
            if (tip == null)
            {
                ShowAlertDanger($"Tip not found with ID <strong>{viewModel.DailyTipId}</strong>.");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var originalFileName = Path.GetFileName(viewModel.ImageFile.FileName);

                var extension = Path.GetExtension(originalFileName);

                var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);

                var image = new DailyLiteracyTipImage
                {
                    DailyLiteracyTipId = viewModel.DailyTipId,
                    Extension = extension,
                    Name = nameWithoutExtension
                };

                await _dailyLiteracyTipService.AddImageAsync(image);

                ShowAlertSuccess("Image added!");
                return RedirectToAction(nameof(Detail), new { tipId = viewModel.DailyTipId });
            }
            catch (GraException gex)
            {
                ShowAlertDanger($"Error: {gex.Message}");
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId, int tipId)
        {
            try
            {
                await _dailyLiteracyTipService.RemoveImageAsync(imageId);
                ShowAlertSuccess("Image deleted.");
            }
            catch (GraException gex)
            {
                ShowAlertDanger($"Error deleting image: {gex.Message}");
            }

            return RedirectToAction(nameof(Detail), new { tipId });
        }

        [HttpPost]
        public async Task<IActionResult> MoveImageUp(int id)
        {
            try
            {
                await _dailyLiteracyTipService.MoveImageUpAsync(id);
                return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }

        [HttpPost]
        public async Task<IActionResult> MoveImageDown(int id)
        {
            try
            {
                await _dailyLiteracyTipService.MoveImageDownAsync(id);
                return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }
    }
}

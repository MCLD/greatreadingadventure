using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.DailyTips;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using SixLabors.ImageSharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageDailyLiteracyTips)]
    public class DailyTipsController : Base.MCController
    {
        private const int LargeImageSize = 600;
        private const long MaxFileSize = 100L * 1024L * 1024L;

        private readonly DailyLiteracyTipService _dailyLiteracyTipService;

        public DailyTipsController(ServiceFacade.Controller context,
            DailyLiteracyTipService dailyLiteracyTipService)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(dailyLiteracyTipService);

            _dailyLiteracyTipService = dailyLiteracyTipService;

            PageTitle = "Daily Tips";
        }

        public static string Name
        { get { return "DailyTips"; } }

        [HttpGet]
        public async Task<IActionResult> Add(int tipId)
        {
            var tip = await _dailyLiteracyTipService.GetByIdAsync(tipId);

            if (tip == null)
            {
                ShowAlertDanger($"Tip not found with ID <strong>{tipId}</strong>.");
                return RedirectToAction(nameof(Index));
            }

            var allowedExtensions = string.Join(",", ValidFiles.ImageExtensions);

            return View(new TipImageAddViewModel
            {
                DailyTipId = tipId,
                AllowedExtensions = allowedExtensions
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(TipImageAddViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

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

            var extension = Path.GetExtension(viewModel.ImageFile.FileName);
            if (!ValidFiles.ImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(viewModel.ImageFile), "Unsupported image file type.");
                return View(viewModel);
            }

            try
            {
                var originalFileName = Path.GetFileName(viewModel.ImageFile.FileName);
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                var newName = nameWithoutExtension;

                int counter = 1;
                while (await _dailyLiteracyTipService.ImageNameExistsAsync(viewModel.DailyTipId, newName, extension))
                {
                    newName = $"{nameWithoutExtension}-{counter++}";
                }

                var image = new DailyLiteracyTipImage
                {
                    DailyLiteracyTipId = viewModel.DailyTipId,
                    Extension = extension,
                    Name = newName
                };

                await _dailyLiteracyTipService.AddImageAsync(image, viewModel.ImageFile);

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
        public async Task<IActionResult> DeleteImage(int imageId, int tipId, int page)
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

            return RedirectToAction(nameof(Detail), new { tipId, page });
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

            var model = new TipDetailViewModel
            {
                CurrentPage = page,
                ItemCount = imageData.Count,
                ItemsPerPage = filter.Take.Value
            };

            if (model.PastMaxPage)
            {
                return RedirectToRoute(new { page = model.LastPage ?? 1 });
            }

            model.Tip = await _dailyLiteracyTipService.GetByIdAsync(tipId);
            var site = await GetCurrentSiteAsync();

            ((List<DailyLiteracyTipImage>)model.Images).AddRange(imageData.Data);

            foreach (var image in model.Images)
            {
                model.Paths.Add(image.Id,
                    _pathResolver.ResolveContentPath($"/site{site.Id}/dailyimages/dailyliteracytip{model.Tip.Id}/{image.Name}{image.Extension}"));
            }

            model.FirstAndLast = await _dailyLiteracyTipService.GetFirstLastDayAsync(tipId);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page)
        {
            page = page == 0 ? 1 : page;

            var filter = new BaseFilter(page);

            var tips = await _dailyLiteracyTipService.GetPaginatedListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                CurrentPage = page,
                ItemCount = tips.Count,
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

        [HttpPost]
        public async Task<JsonResult> MoveImageDown([FromBody] int id)
        {
            return await MoveImageAsync(id, false);
        }

        [HttpPost]
        public async Task<JsonResult> MoveImageUp([FromBody] int id)
        {
            return await MoveImageAsync(id, true);
        }

        [HttpPost]
        public async Task<IActionResult> SetImageDay(int imageId, int tipId, int newDay, int page)
        {
            try
            {
                await _dailyLiteracyTipService.SetImageDayAsync(imageId, newDay);
                ShowAlertSuccess("Image day updated.");
            }
            catch (GraException gex)
            {
                ShowAlertDanger($"Error setting day: {gex.Message}");
            }

            return RedirectToAction(nameof(Detail), new { tipId, page });
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
                DailyLiteracyTip dailyTip = null;

                try
                {
                    using var archive = new ZipArchive(viewmodel.UploadedFile.OpenReadStream());

                    bool isLarge = false;

                    var firstImageEntry = archive.Entries.Where(e => !string.IsNullOrEmpty(e.Name)
                        && ValidFiles.ImageExtensions.Contains(Path.GetExtension(e.Name),
                        StringComparer.OrdinalIgnoreCase))
                        .OrderBy(e => e.Name)
                        .FirstOrDefault();

                    if (firstImageEntry != null)
                    {
                        await using var imageStream = firstImageEntry.Open();
                        using var image = await Image.LoadAsync(imageStream);
                        if (image.Width > LargeImageSize)
                        {
                            isLarge = true;
                        }
                    }

                    dailyTip = await _dailyLiteracyTipService.AddAsync(new DailyLiteracyTip
                    {
                        IsLarge = isLarge,
                        Message = viewmodel.Message,
                        Name = viewmodel.Name,
                    });

                    if (dailyTip == null)
                    {
                        ShowAlertDanger("Unable to create Daily Tip in the database.");
                        return RedirectToAction(nameof(Upload));
                    }

                    var (added, issues) = await _dailyLiteracyTipService.AddImagesZipAsync(dailyTip.Id, archive);

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
                    if (dailyTip != null)
                    {
                        await _dailyLiteracyTipService.RemoveAsync(dailyTip.Id);
                    }
                }
            }
            return RedirectToAction(nameof(Upload));
        }

        private async Task<JsonResult> MoveImageAsync(int id, bool up)
        {
            var image = await _dailyLiteracyTipService.GetImageByIdAsync(id);
            if (image == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid daily literacy tip id."
                });
            }

            var minMax = await _dailyLiteracyTipService
                .GetFirstLastDayAsync(image.DailyLiteracyTipId);

            if (minMax == null)
            {
                return Json(new
                {
                    success = false,
                    message = "This sequence has no images."
                });
            }

            if (up && image.Day == minMax.Item1)
            {
                return Json(new
                {
                    success = false,
                    message = "This is already the first image."
                });
            }
            else if (!up && image.Day == minMax.Item2)
            {
                return Json(new
                {
                    success = false,
                    message = "This is already the last image."
                });
            }

            if (up)
            {
                await _dailyLiteracyTipService.MoveImageUpAsync(id);
            }
            else
            {
                await _dailyLiteracyTipService.MoveImageDownAsync(id);
            }

            return Json(new { success = true });
        }
    }
}

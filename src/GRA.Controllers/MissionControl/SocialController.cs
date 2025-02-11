using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Social;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageSocial)]
    public class SocialController : Base.MCController
    {
        private const long MaxFileSize = 512L * 1024L;

        private readonly LanguageService _languageService;
        private readonly SocialManagementService _socialManagementService;
        private readonly SocialService _socialService;

        public SocialController(ServiceFacade.Controller context,
            LanguageService languageService,
            SocialManagementService socialManagementService,
            SocialService socialService) : base(context)
        {
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(socialManagementService);
            ArgumentNullException.ThrowIfNull(socialService);

            _languageService = languageService;
            _socialManagementService = socialManagementService;
            _socialService = socialService;

            PageTitle = "Social";
        }

        public static string Name
        { get { return "Social"; } }

        [HttpGet]
        public async Task<IActionResult> AddSocialHeader()
        {
            var viewmodel = new SocialItemViewModel
            {
                LanguageList = new SelectList(await _languageService.GetActiveAsync(),
                    "Id",
                    "Description")
            };

            if (viewmodel.StartDate == default)
            {
                viewmodel.StartDate = _dateTimeProvider.Now;
            }

            return View(viewmodel);
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "Show the user a friendly error rather than an exception page")]
        public async Task<IActionResult> AddSocialHeader(SocialItemViewModel viewmodel)
        {
            if (viewmodel == null)
            {
                ShowAlertDanger("Unable to add social header - object was empty.");
                return RedirectToAction(nameof(AddSocialHeader));
            }
            byte[] imageBytes = null;

            // check filesize
            if (viewmodel.UploadedImage?.Length > MaxFileSize)
            {
                ModelState.AddModelError(nameof(viewmodel.UploadedImage),
                    $"Uploaded file is too large, please upload a file smaller than {MaxFileSize:N0} bytes");
            }
            else
            {
                try
                {
                    await using var ms = new System.IO.MemoryStream();
                    await viewmodel.UploadedImage.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(viewmodel.UploadedImage), ex.Message);
                }
            }

            if (!ModelState.IsValid)
            {
                viewmodel.LanguageList = new SelectList(await _languageService.GetActiveAsync(),
                    "Id",
                    "Description");
                return View(viewmodel);
            }

            var header = new SocialHeader
            {
                Name = viewmodel.Name,
                StartDate = viewmodel.StartDate,
                SiteId = GetCurrentSiteId(),
                Socials = new List<Social>
                {
                    new Social
                    {
                        Description = viewmodel.Description,
                        ImageAlt = viewmodel.ImageAlt,
                        LanguageId = viewmodel.LanguageId,
                        Title = viewmodel.Title,
                        TwitterUsername = viewmodel.TwitterUsername
                   }
                }
            };

            var headerId = await _socialManagementService.AddHeaderAndSocialAsync(header,
                viewmodel.UploadedImage.FileName,
                imageBytes);

            TempData[$"RowStatus{headerId}"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSocial(int headerId, int languageId)
        {
            var deletedId = await _socialManagementService.DeleteSocial(headerId, languageId);

            if (deletedId != null)
            {
                TempData[$"RowStatus{deletedId}"] = "warning";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page)
        {
            page = page == 0 ? 1 : page;

            var filter = new BaseFilter(page);

            var viewModel = new SocialListViewModel
            {
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value,
            };

            if (viewModel.PastMaxPage)
            {
                return RedirectToRoute(new { page = viewModel.LastPage ?? 1 });
            }

            ((List<Language>)viewModel.Languages).AddRange(await _languageService.GetActiveAsync());

            var socialHeaders = await _socialManagementService.GetPaginatedListAsync(filter);
            ((List<SocialHeader>)viewModel.SocialHeaders).AddRange(socialHeaders.Data);
            viewModel.ItemCount = socialHeaders.Count;

            foreach (var language in viewModel.Languages)
            {
                var current = await _socialService.GetAsync(language.Name);
                if (current != null)
                {
                    viewModel.ActiveSocials.Add(new ActiveSocials
                    {
                        Badge = language.Description,
                        HeaderId = current.SocialHeaderId,
                        Link = Url.Action(nameof(ViewSocial), new
                        {
                            socialHeaderId = current.SocialHeaderId,
                            languageId = language.Id
                        })
                    });
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "Show the user a friendly error rather than an exception page")]
        public async Task<IActionResult> ReplaceImage(ReplaceImageViewModel viewmodel)

        {
            if (viewmodel == null)
            {
                ShowAlertDanger("Could not find the social item for image replacement.");
                return RedirectToAction(nameof(Index));
            }

            var social = await _socialManagementService.GetHeaderAndSocialAsync(viewmodel.HeaderId,
                viewmodel.LanguageId);

            if (social == null)
            {
                ShowAlertDanger("Could not find the social item for image replacement.");
                return RedirectToAction(nameof(Index));
            }

            byte[] imageBytes = null;

            try
            {
                await using var ms = new System.IO.MemoryStream();
                await viewmodel.UploadedImage.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(viewmodel.UploadedImage), ex.Message);
            }

            if (ModelState.IsValid)
            {
                await _socialManagementService.ReplaceImageAsync(viewmodel.HeaderId,
                    viewmodel.LanguageId,
                    viewmodel.UploadedImage.FileName,
                    imageBytes);
            }

            return RedirectToAction(nameof(ViewSocial), new
            {
                socialHeaderId = viewmodel.HeaderId,
                languageId = viewmodel.LanguageId
            });
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "Show the user a friendly error rather than an exception page")]
        public async Task<IActionResult> UpdateSocial(SocialItemViewModel viewmodel)
        {
            if (viewmodel == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var header = await _socialManagementService.GetHeaderAndSocialAsync(viewmodel.HeaderId,
                viewmodel.LanguageId);

            if (header.Name != viewmodel.Name || header.StartDate != viewmodel.StartDate)
            {
                await _socialManagementService.UpdateHeaderAsync(viewmodel.HeaderId,
                    viewmodel.Name,
                    viewmodel.StartDate);
            }

            var social = new Social
            {
                Description = viewmodel.Description,
                ImageAlt = viewmodel.ImageAlt,
                LanguageId = viewmodel.LanguageId,
                SocialHeaderId = viewmodel.HeaderId,
                Title = viewmodel.Title,
                TwitterUsername = viewmodel.TwitterUsername
            };

            if (viewmodel.SocialIsNew)
            {
                byte[] imageBytes = null;

                try
                {
                    await using var ms = new System.IO.MemoryStream();
                    await viewmodel.UploadedImage.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(viewmodel.UploadedImage), ex.Message);
                }

                if (!ModelState.IsValid)
                {
                    return RedirectToAction(nameof(ViewSocial), new
                    {
                        socialHeaderId = viewmodel.HeaderId,
                        languageId = viewmodel.LanguageId
                    });
                }

                await _socialManagementService.AddSocialAsync(social,
                    viewmodel.UploadedImage.FileName,
                    imageBytes);
            }
            else
            {
                await _socialManagementService.UpdateSocialAsync(social);
            }

            TempData[$"RowStatus{viewmodel.HeaderId}"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ViewSocial(int socialHeaderId, int languageId)
        {
            var socialHeader = await _socialManagementService
                .GetHeaderAndSocialAsync(socialHeaderId, languageId);

            if (socialHeader == null)
            {
                return RedirectToAction(nameof(AddSocialHeader));
            }

            var language = await _languageService.GetActiveByIdAsync(languageId);

            var social = socialHeader.Socials?.FirstOrDefault();

            var viewmodel = new SocialItemViewModel
            {
                Description = social?.Description,
                HeaderId = socialHeaderId,
                ImageAlt = social?.ImageAlt,
                ImageLink = social?.ImageLink,
                LanguageId = languageId,
                LanguageName = language.Description,
                Name = socialHeader.Name,
                SocialIsNew = social == null,
                StartDate = socialHeader.StartDate,
                Title = social?.Title,
                TwitterUsername = social?.TwitterUsername
            };

            if (social?.ImageLink != null
                && social.ImageLink.Contains('/', StringComparison.OrdinalIgnoreCase))
            {
                viewmodel.Filename = social.ImageLink[social.ImageLink.LastIndexOf('/')..];
            }

            if (social?.ImageWidth != default && social?.ImageHeight != default)
            {
                viewmodel.ImageDimensions = $"{social?.ImageWidth} x {social?.ImageHeight}";
            }

            return View(viewmodel);
        }
    }
}

using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Text;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers.MissionControl
{
    [Area(nameof(MissionControl))]
    [Authorize(Policy = Policy.ManageSites)]
    public class TextController : Base.MCController
    {
        private readonly LanguageService _languageService;
        private readonly SegmentService _segmentService;
        private readonly SiteService _siteService;

        public TextController(ServiceFacade.Controller context,
            LanguageService languageService,
            SegmentService segmentService,
            SiteService siteService) : base(context)
        {
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(segmentService);
            ArgumentNullException.ThrowIfNull(siteService);

            _languageService = languageService;
            _segmentService = segmentService;
            _siteService = siteService;

            PageTitle = "Update text";
        }

        public static string Area
        { get { return nameof(MissionControl); } }

        public static string Name
        { get { return "Text"; } }

        [HttpPost]
        public async Task<IActionResult> ClearSetting(int deleteTextId, string deleteSettingKey)
        {
            ArgumentNullException.ThrowIfNull(deleteSettingKey);

            await _siteService.UpdateSiteSettingAsync(GetCurrentSiteId(),
                deleteSettingKey,
                string.Empty);

            await _segmentService.RemoveSegmentAsync(deleteTextId);

            return Redirect(GetSiteSettingsLink());
        }

        [HttpGet]
        public async Task<IActionResult> CreateSetting(string id)
        {
            return View("Update", new UpdateTextViewModel
            {
                CurrentLanguage = await _languageService.GetDefaultLanguageIdAsync(),
                DisplayName = GetSiteSettingDisplayName(id),
                FormAction = nameof(CreateSetting),
                ReturnLink = GetSiteSettingsLink(),
                SegmentName = id,
                SegmentType = SegmentType.SiteSetting
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSetting(UpdateTextViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            var segment = await _segmentService.AddTextAsync(GetActiveUserId(),
                viewModel.CurrentLanguage,
                SegmentType.SiteSetting,
                viewModel.SegmentText,
                viewModel.SegmentName);

            await _siteService.UpdateSiteSettingAsync(GetCurrentSiteId(),
                viewModel.SegmentName,
                segment.SegmentId.ToString(CultureInfo.InvariantCulture));

            return Redirect(viewModel.ReturnLink);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, int? languageId)
        {
            var lookupLanguageId = languageId ?? await _languageService.GetDefaultLanguageIdAsync();

            var segment = await _segmentService.GetAsync(id);

            if (segment == null)
            {
                ShowAlertDanger("Text not found.");
                return RedirectToAction(nameof(HomeController.Index),
                    HomeController.Name,
                    new { area = HomeController.Area });
            }

            var segmentLanguages = await _segmentService.GetLanguageStatusAsync([id]);
            var segmentText = await _segmentService.GetDbTextAsync(id, lookupLanguageId);

            var viewModel = new UpdateTextViewModel
            {
                CurrentLanguage = lookupLanguageId,
                Id = id,
                FormAction = nameof(Update),
                SegmentLanguages = segmentLanguages[id],
                SegmentName = segment.Name,
                SegmentText = segmentText,
                SegmentType = segment.SegmentType
            };

            switch (segment.SegmentType)
            {
                case SegmentType.SiteSetting:
                    viewModel.DisplayName = GetSiteSettingDisplayName(segment.Name);
                    viewModel.ReturnLink = GetSiteSettingsLink();
                    break;
            }

            foreach (var language in await _languageService.GetIdDescriptionDictionaryAsync())
            {
                viewModel.Languages.Add(language.Key, language.Value);
            }

            return View("Update", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateTextViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            // modelviewstate
            if (ModelState.IsValid)
            {
                await _segmentService.UpdateTextAsync(viewModel.Id.Value,
                    viewModel.CurrentLanguage,
                    viewModel.SegmentText?.Trim());
                return RedirectToAction(nameof(Update),
                    new { id = viewModel.Id.Value, languageId = viewModel.CurrentLanguage });
            }
            else
            {
                // we already have: CurrentLanguage, Id, SegmentText
                var segment = await _segmentService.GetAsync(viewModel.Id.Value);
                var segmentLanguages = await _segmentService
                    .GetLanguageStatusAsync([viewModel.Id.Value]);
                viewModel.FormAction = nameof(Update);
                viewModel.SegmentLanguages = segmentLanguages[viewModel.Id.Value];
                viewModel.SegmentName = segment.Name;
                viewModel.SegmentType = segment.SegmentType;
                foreach (var language in await _languageService.GetIdDescriptionDictionaryAsync())
                {
                    viewModel.Languages.Add(language.Key, language.Value);
                }

                var sb = new StringBuilder("There were issues with your submission:<ul class=\"mb-0\">");
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        sb.Append("<li>").Append(error.ErrorMessage).AppendLine("</li>");
                    }
                }
                sb.Append("</ul>");

                ShowAlertDanger(sb.ToString()?.Trim());

                return View("Update", viewModel);
            }
        }

        private static string GetSiteSettingDisplayName(string siteSetting)
        {
            var siteSettingInfo = SiteSettingDefinitions.DefinitionDictionary[siteSetting];
            return $"Site setting: {siteSettingInfo.Category}/{siteSettingInfo.Name}";
        }

        private string GetSiteSettingsLink()
        {
            return Url.Action(nameof(SitesController.Settings),
                SitesController.Name,
                new
                {
                    area = SitesController.Area,
                    id = GetCurrentSiteId()
                });
        }
    }
}

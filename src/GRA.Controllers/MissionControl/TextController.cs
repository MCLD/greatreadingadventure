using System;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Text;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers.MissionControl
{
    [Area(nameof(MissionControl))]
    public class TextController : Base.MCController
    {
        private readonly LanguageService _languageService;
        private readonly SegmentService _segmentService;

        public TextController(ServiceFacade.Controller context,
            LanguageService languageService,
            SegmentService segmentService) : base(context)
        {
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(segmentService);

            _languageService = languageService;
            _segmentService = segmentService;

            PageTitle = "Update text";
        }

        public static string Name
        { get { return "Text"; } }

        [HttpGet]
        public async Task<IActionResult> Preview(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, int? languageId)
        {
            var lookupLanguageId = languageId
                ?? await _languageService.GetDefaultLanguageIdAsync();

            string segmentName;

            try
            {
                segmentName = await _segmentService.GetNameAsync(id);
            }
            catch (GraException)
            {
                ShowAlertDanger("Text not found.");
                return RedirectToAction(nameof(HomeController.Index),
                    HomeController.Name,
                    new { area = HomeController.Area });
            }

            var segmentLanguages = await _segmentService.GetLanguageStatusAsync([id]);
            var segmentText = await _segmentService.GetTextAsync(id, lookupLanguageId, true);

            var viewModel = new UpdateTextViewModel
            {
                CurrentLanguage = lookupLanguageId,
                Id = id,
                SegmentLanguages = segmentLanguages[id],
                SegmentName = segmentName,
                SegmentText = segmentText
            };

            foreach (var language in await _languageService.GetIdDescriptionDictionaryAsync())
            {
                viewModel.Languages.Add(language.Key, language.Value);
            }

            return View("Update", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateTextViewModel viewModel)
        {
            // modelviewstate
            throw new NotImplementedException();
        }
    }
}

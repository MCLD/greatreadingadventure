using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.ViewModel.MissionControl.Social
{
    public class ReplaceImageViewModel
    {
        public int HeaderId { get; set; }
        public int LanguageId { get; set; }
        public IFormFile UploadedImage { get; set; }
    }
}
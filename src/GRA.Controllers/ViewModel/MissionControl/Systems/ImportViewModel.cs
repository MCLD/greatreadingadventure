using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.ViewModel.MissionControl.Systems
{
    public class ImportViewModel
    {
        public bool DoImport { get; set; }
        public IFormFile FileUpload { get; set; }
        public int SiteId { get; set; }
    }
}

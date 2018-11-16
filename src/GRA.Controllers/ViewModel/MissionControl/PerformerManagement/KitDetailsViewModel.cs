using System.Collections.Generic;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class KitDetailsViewModel
    {
        public PsKit Kit { get; set; }
        public bool NewKit { get; set; }

        public SelectList AgeList { get; set; }
        public List<int> AgeSelection { get; set; }
        public List<IFormFile> Images { get; set; }
        public int MaxUploadMB { get; set; }
    }
}

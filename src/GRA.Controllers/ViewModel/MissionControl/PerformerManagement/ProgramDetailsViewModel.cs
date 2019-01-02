using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class ProgramDetailsViewModel
    {
        public PsProgram Program { get; set; }

        public SelectList AgeList { get; set; }
        public List<int> AgeSelection { get; set; }
        public List<IFormFile> Images { get; set; }

        public int? PerformerId { get; set; }
        public string PerformerName { get; set; }

        public int MaxUploadMB { get; set; }
    }
}

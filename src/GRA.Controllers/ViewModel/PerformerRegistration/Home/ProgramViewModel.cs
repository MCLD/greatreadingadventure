using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.PerformerRegistration.Home
{
    public class ProgramViewModel
    {
        public PsProgram Program { get; set; }

        public SelectList AgeList { get; set; }
        public List<int> AgeSelection { get; set; }

        public List<IFormFile> Images { get; set; }
        public int MaxUploadMB { get; set; }

        public bool RegistrationCompleted { get; set; }
        public bool EditingProgram { get; set; }
    }
}

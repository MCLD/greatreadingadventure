using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.ViewModel.PerformerRegistration.Home
{
    public class PerformerImagesViewModel
    {
        public bool IsEditable { get; set; }
        public List<PsPerformerImage> PerformerImages { get; set; }
        public List<IFormFile> Images { get; set; }
        public string ImagesToDelete { get; set; }
        public int MaxUploadMB { get; set; }
    }
}

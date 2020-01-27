using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.EmailManagement
{
    public class EmailAddressesViewModel
    {
        public IFormFile UploadedFile { get; set; }

        public bool IsAdmin { get; set; }

        public SelectList SignUpSources { get; set; }

        public string SignUpSource { get; set; }
    }
}

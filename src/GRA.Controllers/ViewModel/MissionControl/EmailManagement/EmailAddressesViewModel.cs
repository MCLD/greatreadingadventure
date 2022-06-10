using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.EmailManagement
{
    public class EmailAddressesViewModel
    {
        private static IEnumerable<SelectListItem> _allSources;

        public static IEnumerable<SelectListItem> AllSources
        {
            get
            {
                if (_allSources == null)
                {
                    _allSources = new List<SelectListItem> {
                    new SelectListItem("Before Registration", nameof(SiteStage.BeforeRegistration)),
                    new SelectListItem("Access Closed", nameof(SiteStage.AccessClosed))
                    };
                }
                return _allSources;
            }
        }

        public bool HasSources { get; set; }
        public string SignUpSource { get; set; }

        [Required(ErrorMessage = "Please select a source to download")]
        public SelectList SignUpSources { get; set; }

        public IFormFile UploadedFile { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.EmailManagement
{
    public class EmailAddressesViewModel
    {
        public IFormFile UploadedFile { get; set; }

        [Required(ErrorMessage = "Please select a source to download")]
        public SelectList SignUpSources { get; set; }

        public string SignUpSource { get; set; }
        public bool HasSources { get; set; }
    }
}

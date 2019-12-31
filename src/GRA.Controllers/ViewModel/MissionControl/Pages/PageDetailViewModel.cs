using System.ComponentModel;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Pages
{
    public class PageDetailViewModel
    {
        public Page Page { get; set; }
        public int HeaderId { get; set; }

        [DisplayName("Page Name")]
        public string HeaderName { get; set; }

        [DisplayName("Stub")]
        public string HeaderStub { get; set; }

        public bool NewPage { get; set; }
        public int SelectedLanguageId { get; set; }
        public string PageUrl { get; set; }

        public SelectList LanguageList { get; set; }
    }
}

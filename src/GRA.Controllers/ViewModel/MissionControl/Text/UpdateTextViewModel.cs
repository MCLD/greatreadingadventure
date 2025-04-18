using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GRA.Controllers.ViewModel.MissionControl.Text
{
    public class UpdateTextViewModel
    {
        public UpdateTextViewModel()
        {
            Languages = new Dictionary<int, string>();
        }

        public int CurrentLanguage { get; set; }
        public int? Id { get; set; }
        public IDictionary<int, string> Languages { get; }
        public string ReturnLink { get; set; }

        public IEnumerable<int> SegmentLanguages { get; set; }

        [DisplayName("Name of text")]
        public string SegmentName { get; set; }

        [DisplayName("Text")]
        [Required]
        public string SegmentText { get; set; }

        public string LanguageButtonClass(int languageId)
        {
            return languageId == CurrentLanguage
                ? "btn-success"
                : SegmentLanguages.Contains(languageId)
                    ? "btn-outline-success"
                    : "btn-outline-warning";
        }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Join
{
    public class Step2ViewModel : SchoolSelectionViewModel
    {
        [Required(ErrorMessage = Annotations.RequiredSelection)]
        [DisplayName("Program")]
        [Range(0, int.MaxValue, ErrorMessage = Annotations.RequiredProgramSelection)]
        public int? ProgramId { get; set; }

        public int? Age { get; set; }

        public bool ShowAge { get; set; }
        public bool ShowSchool { get; set; }
        public string ProgramJson { get; set; }

        public SelectList ProgramList { get; set; }
    }
}

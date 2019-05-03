using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Join
{
    public class Step2ViewModel : SchoolSelectionViewModel
    {
        [Required(ErrorMessage = ErrorMessages.Selection)]
        [DisplayName(DisplayNames.Program)]
        [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.FieldProgram)]
        public int? ProgramId { get; set; }

        [DisplayName(DisplayNames.Age)]
        public int? Age { get; set; }

        public bool ShowAge { get; set; }
        public bool ShowSchool { get; set; }
        public string ProgramJson { get; set; }

        public SelectList ProgramList { get; set; }
    }
}

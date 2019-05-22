using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class HouseholdImportViewModel : SchoolSelectionViewModel
    {
        public int Id { get; set; }

        [DisplayName("System")]
        [Required]
        public int? SystemId { get; set; }

        [DisplayName("Branch")]
        [Required]
        public int? BranchId { get; set; }

        [DisplayName("Program")]
        [Required]
        public int? ProgramId { get; set; }

        [DisplayName("Is this their first time participating?")]
        [Required(ErrorMessage = "Please let us know if this is their first time participating in the program.")]
        public string IsFirstTime { get; set; }

        [DisplayName("Excel File")]
        [Required]
        public IFormFile UserExcelFile { get; set; }

        public bool ShowSchool { get; set; }
        public string ProgramJson { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList ProgramList { get; set; }
        public SelectList SystemList { get; set; }
        public SelectList AskFirstTime { get; set; }
    }
}

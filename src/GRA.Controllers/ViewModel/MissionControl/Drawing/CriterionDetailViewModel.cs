using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Drawing
{
    public class CriterionDetailViewModel
    {
        public GRA.Domain.Model.DrawingCriterion Criterion { get; set; }
        public bool CanViewParticipants { get; set; }
        public string CreatedByName { get; set; }
        public SelectList SystemList { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList ProgramList { get; set; }
        [DisplayName("Require participant to have read a book")]
        public string ProgramPlaceholder { get; set; }

        public int EligibleCount { get; set; }
    }
}

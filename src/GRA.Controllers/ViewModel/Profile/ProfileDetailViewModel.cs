using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Profile
{
    public class ProfileDetailViewModel
    {
        public Domain.Model.User User { get; set; }
        public int HouseholdCount { get; set; }
        public bool HasAccount { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList ProgramList { get; set; }
        public SelectList SystemList { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class ParticipantsDetailViewModel
    {
        public GRA.Domain.Model.User User { get; set; }
        public int Id { get; set; }
        public string Username { get; set; }
        public int HouseholdCount { get; set; }
        public int PrizeCount { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public bool HasAccount { get; set; }
        public bool CanEditDetails { get; set; }
        public bool CanEditUsername { get; set; }
        public bool RequirePostalCode { get; set; }
        public bool ShowAge { get; set; }
        public bool ShowSchool { get; set; }
        public bool HasSchoolId { get; set; }
        public bool NewEnteredSchool { get; set; }
        public int? SchoolDistrictId { get; set; }
        public int? SchoolTypeId { get; set; }
        public string ProgramJson { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList ProgramList { get; set; }
        public SelectList SystemList { get; set; }
        public SelectList SchoolList { get; set; }
        public SelectList SchoolDistrictList { get; set; }
        public SelectList SchoolTypeList { get; set; }
    }
}

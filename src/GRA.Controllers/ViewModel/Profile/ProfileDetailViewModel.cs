using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Profile
{
    public class ProfileDetailViewModel
    {
        public Domain.Model.User User { get; set; }
        public int HouseholdCount { get; set; }
        public bool HasAccount { get; set; }
        public bool RequirePostalCode { get; set; }
        public bool ShowAge { get; set; }
        public bool ShowSchool { get; set; }
        public bool HasSchoolId { get; set; }
        public bool NewEnteredSchool { get; set; }
        public int? SchoolDistrictId { get; set; }
        public int? SchoolTypeId { get; set; }
        public string ProgramJson { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList SystemList { get; set; }
        public SelectList ProgramList { get; set; }
        public SelectList SchoolList { get; set; }
        public SelectList SchoolDistrictList { get; set; }
        public SelectList SchoolTypeList { get; set; }
        public bool RestrictChangingSystemBranch { get; set; }
        public string SystemName { get; set; }
        public string BranchName { get; set; }
    }
}

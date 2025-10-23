using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Profile
{
    public class ProfileDetailViewModel : SchoolSelectionViewModel
    {
        public string ActivityDescriptionPlural { get; set; }
        public bool AskEmailSubscription { get; set; }
        public string AskEmailSubscriptionText { get; set; }
        public bool AskPersonalPointGoal { get; set; }
        public SelectList BranchList { get; set; }
        public string BranchName { get; set; }
        public EmailAwardViewModel EmailAwardModel { get; set; }
        public bool HasAccount { get; set; }
        public int HouseholdCount { get; set; }
        public int? MaximumPersonalPointGoal { get; set; }
        public int? MinimumPersonalPointGoal { get; set; }
        public string ProgramJson { get; set; }
        public SelectList ProgramList { get; set; }
        public string ProgramName { get; set; }
        public bool RequirePostalCode { get; set; }
        public bool RestrictChangingProgram { get; set; }
        public bool RestrictChangingSystemBranch { get; set; }
        public bool ShowAge { get; set; }
        public bool ShowSchool { get; set; }
        public SelectList SystemList { get; set; }
        public string SystemName { get; set; }
        public string TranslationDescriptionPastTense { get; set; }
        public Domain.Model.User User { get; set; }
    }
}

using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class ParticipantsDetailViewModel : ParticipantPartialViewModel
    {
        public ParticipantsDetailViewModel()
        {
        }

        public ParticipantsDetailViewModel(ParticipantPartialViewModel viewModel) : base(viewModel)
        {
        }

        public string ActivityDescriptionPlural { get; set; }
        public bool AskEmailSubscription { get; set; }
        public string AskEmailSubscriptionText { get; set; }
        public bool AskPersonalPointGoal { get; set; }
        public SelectList BranchList { get; set; }
        public bool CanEditDetails { get; set; }
        public bool CanEditUsername { get; set; }
        public bool CanViewParticipants { get; set; }
        public string CreatedByName { get; set; }
        public EmailAwardViewModel EmailAwardModel { get; set; }
        public int? HeadOfHouseholdId { get; set; }

        public bool IsVendorCodeDisplayed
        {
            get
            {
                return !string.IsNullOrWhiteSpace(User.VendorCode)
                    || User.NeedsToAnswerVendorCodeQuestion
                    || User.Donated == true;
            }
        }

        public int? MaximumPersonalPointGoal { get; set; }
        public int? MinimumPersonalPointGoal { get; set; }
        public string ProgramJson { get; set; }
        public SelectList ProgramList { get; set; }
        public bool RequirePostalCode { get; set; }
        public School School { get; set; }
        public bool ShowAge { get; set; }
        public bool ShowSchool { get; set; }
        public SelectList SystemList { get; set; }
        public string TranslationDescriptionPastTense { get; set; }
        public User User { get; set; }
        public string Username { get; set; }
    }
}

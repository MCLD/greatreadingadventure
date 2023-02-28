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
        public SelectList BranchList { get; set; }
        public bool CanEditDetails { get; set; }
        public bool CanEditUsername { get; set; }
        public EmailAwardViewModel EmailAwardModel { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public string ProgramJson { get; set; }
        public SelectList ProgramList { get; set; }
        public bool RequirePostalCode { get; set; }
        public School School { get; set; }
        public bool ShowAge { get; set; }
        public bool ShowSchool { get; set; }
        public SelectList SystemList { get; set; }
        public string TranslationDescriptionPastTense { get; set; }
        public GRA.Domain.Model.User User { get; set; }
        public string Username { get; set; }
    }
}

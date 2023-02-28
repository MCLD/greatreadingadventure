using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class MailListViewModel : ParticipantPartialViewModel
    {
        public MailListViewModel()
        {
        }

        public MailListViewModel(ParticipantPartialViewModel viewModel) : base(viewModel)
        {
        }

        public bool CanRemoveMail { get; set; }
        public bool CanSendMail { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public IEnumerable<GRA.Domain.Model.Mail> Mails { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}

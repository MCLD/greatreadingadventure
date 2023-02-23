using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class EmailSubscriptionLogViewModel : ParticipantPartialViewModel
    {
        public EmailSubscriptionLogViewModel()
        {
        }

        public EmailSubscriptionLogViewModel(ParticipantPartialViewModel viewModel) : base(viewModel)
        {
        }

        public ICollection<EmailSubscriptionAuditLog> SubscritionAuditLogs { get; set; }
    }
}

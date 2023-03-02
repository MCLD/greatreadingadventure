using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class ChangeHistoryViewModel : ParticipantPartialViewModel
    {
        public ChangeHistoryViewModel()
        {
        }

        public ChangeHistoryViewModel(ParticipantPartialViewModel viewModel) : base(viewModel)
        {
        }

        public IEnumerable<ChangedItem<User>> ChangedItems { get; set; }
    }
}

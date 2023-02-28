using System.ComponentModel;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class LogActivityViewModel : ParticipantPartialViewModel
    {
        public LogActivityViewModel()
        {
        }

        public LogActivityViewModel(ParticipantPartialViewModel viewModel) : base(viewModel)
        {
        }

        [DisplayName("Activity Amount")]
        public int? ActivityAmount { get; set; }

        public bool DisableSecretCode { get; set; }
        public bool HasPendingQuestionnaire { get; set; }
        public bool IsSecretCode { get; set; }

        public PointTranslation PointTranslation { get; set; }

        [DisplayName("Secret Code")]
        public string SecretCode { get; set; }

        public int? VendorCodeTypeId { get; set; }
        public SelectList VendorCodeTypeList { get; set; }
    }
}

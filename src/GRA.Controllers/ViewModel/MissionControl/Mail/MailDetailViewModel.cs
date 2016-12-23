using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Mail
{
    public class MailDetailViewModel
    {
        public GRA.Domain.Model.Mail Mail { get; set; }
        public string SentMessage { get; set; }
        public string ParticipantLink { get; set; }
        public string ParticipantName { get; set; }
        public bool CanDelete { get; set; }
        public bool CanMail { get; set; }
    }
}

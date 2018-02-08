using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Mail
{
    public class MailDetailViewModel
    {
        public GRA.Domain.Model.Mail Mail { get; set; }
        public List<GRA.Domain.Model.Mail> MailThread { get; set; }
        public GRA.Domain.Model.User User { get; set; }
        public string SentMessage { get; set; }
        public bool CanDelete { get; set; }
        public bool CanMail { get; set; }
    }
}

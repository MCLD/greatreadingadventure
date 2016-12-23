using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.MissionControl.Mail
{
    public class MailReplyViewModel
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
        public int? InReplyToId { get; set; }
        public string InReplyToSubject { get; set; }
        public string ParticipantName { get; set; }
        public string ParticipantLink { get; set; }
    }
}

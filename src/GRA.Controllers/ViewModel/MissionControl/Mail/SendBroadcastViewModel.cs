using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.MissionControl.Mail
{
    public class SendBroadcastViewModel
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
    }
}

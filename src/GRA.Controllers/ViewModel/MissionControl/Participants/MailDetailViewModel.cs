namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class MailDetailViewModel
    {
        public GRA.Domain.Model.Mail Mail { get; set; }
        public int Id { get; set; }
        public bool CanRemoveMail { get; set; }
    }
}

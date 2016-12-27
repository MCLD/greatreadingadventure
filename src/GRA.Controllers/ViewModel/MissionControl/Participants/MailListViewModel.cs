using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class MailListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Mail> Mails { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int Id { get; set; }
        public int HouseholdCount { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public bool HasAccount { get; set; }
        public bool CanRemoveMail { get; set; }
        public bool CanSendMail { get; set; }
    }
}

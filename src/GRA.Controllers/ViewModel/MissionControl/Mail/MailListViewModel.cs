using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Mail
{
    public class MailListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Mail> Mail { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public bool CanDelete { get; set; }
    }
}

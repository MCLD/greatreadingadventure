using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Mail
{
    public class MailListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Mail> Mail { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}

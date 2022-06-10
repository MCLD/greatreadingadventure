using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.EmailManagement
{
    public class EmailBaseIndexViewModel : PaginateViewModel
    {
        public ICollection<EmailBase> EmailBases { get; set; }
        public IDictionary<int, string> LanguageNames { get; set; }
    }
}

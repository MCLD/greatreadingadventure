using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.EmailManagement
{
    public class EmailDetailViewModel
    {
        public EmailTemplate EmailTemplate { get; set; }
        public string Action { get; set; }
        public IEnumerable<string> SelectedPermissions { get; set; }
        public IEnumerable<string> UnselectedPermissions { get; set; }
        public string Permissions { get; set; }
    }
}

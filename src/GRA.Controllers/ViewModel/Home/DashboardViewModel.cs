using System.Collections.Generic;
using System.ComponentModel;

namespace GRA.Controllers.ViewModel.Home
{
    public class DashboardViewModel
    {
        public string FirstName { get; set; }
        public int CurrentPointTotal { get; set; }
        public string AvatarPath { get; set; }
        public bool SingleEvent { get; set; }
        public string ActivityDescriptionPlural { get; set; }

        public int? ActivityAmount { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

        [DisplayName("Code")]
        public string SecretCode { get; set; }
        public string SecretCodeMessage { get; set; }

        public IEnumerable<GRA.Domain.Model.Badge> Badges { get; set; }
    }
}

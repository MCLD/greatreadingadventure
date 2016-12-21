using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Home
{
    public class DashboardViewModel
    {
        public string FirstName { get; set; }
        public int CurrentPointTotal { get; set; }
        public string AvatarPath { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }

        public IEnumerable<GRA.Domain.Model.Badge> Badges { get; set; }
    }
}

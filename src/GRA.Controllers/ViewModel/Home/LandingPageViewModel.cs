using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Home
{
    public class LandingPageViewModel
    {
        public string RegistrationOpens { get; set; }
        public bool CollectEmail { get; set; }
        public string Email { get; set; }
        public string SignUpSource { get; set; }
        public string SiteName { get; set; }
        public Social Social { get; set; }
    }
}

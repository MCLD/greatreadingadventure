using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Home
{
    public abstract class BaseLandExitViewModel
    {
        public string BannerAltText { get; set; }
        public string BannerImagePath { get; set; }
        public string LeftMessage { get; set; }
        public Social Social { get; set; }

    }
}

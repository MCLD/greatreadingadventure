namespace GRA.Controllers.ViewModel.MissionControl.Carousels
{
    public class ItemDetailViewModel
    {
        public int CarouselId { get; set; }
        public string CarouselName { get; set; }
        public PageAction PageAction { get; set; }
        public Domain.Model.CarouselItem CarouselItem { get; set; }
    }
}

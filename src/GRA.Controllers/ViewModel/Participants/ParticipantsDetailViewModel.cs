namespace GRA.Controllers.ViewModel.Participants
{
    public class ParticipantsDetailViewModel
    {
        public GRA.Domain.Model.User User { get; set; }
        public int Id { get; set; }
        public int HouseholdCount { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public bool CanEditDetails { get; set; }
    }
}

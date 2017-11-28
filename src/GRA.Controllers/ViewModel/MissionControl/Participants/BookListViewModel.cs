using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class BookListViewModel
    {
        public List<GRA.Domain.Model.Book> Books { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Sort { get; set; }
        public bool IsDescending { get; set; }
        public GRA.Domain.Model.Book Book { get; set; }
        public int Id { get; set; }
        public bool HasPendingQuestionnaire { get; set; }
        public int HouseholdCount { get; set; }
        public int PrizeCount { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public bool HasAccount { get; set; }
        public bool CanEditBooks { get; set; }
        public System.Array SortBooks { get; set; }
    }
}

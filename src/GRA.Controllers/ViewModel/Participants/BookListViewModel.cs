using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Participants
{
    public class BookListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Book> Books { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int Id { get; set; }
        public int HouseholdCount { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public bool CanModifyBooks { get; set; }
    }
}

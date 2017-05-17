using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Profile
{
    public class BookListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Book> Books { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public GRA.Domain.Model.Book Book { get; set; }
        public int HouseholdCount { get; set; }
        public bool HasAccount { get; set; }
        public bool CanEditBooks { get; set; }
    }
}

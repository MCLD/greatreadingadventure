using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Profile
{
    public class AttachmentListViewModel
    {
        public List<AttachmentItemViewModel> Attachments { get; set; }
        public bool HasAccount { get; set; }
        public int HouseholdCount { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public ICollection<UserLog> UserLogs { get; set; }
    }
}

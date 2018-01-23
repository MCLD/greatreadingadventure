using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class UpdateGroupViewModel
    {
        public int HouseholdHeadUserId { get; set; }
        public GroupInfo GroupInfo { get; set; }
        public SelectList GroupTypes { get; set; }
    }
}

using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class GroupUpgradeViewModel
    {
        public int Id { get; set; }
        public int MaximumHouseholdAllowed { get; set; }
        public GroupInfo GroupInfo { get; set; }
        public SelectList GroupTypes { get; set; }
    }
}

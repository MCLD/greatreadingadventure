using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Profile
{
    public class GroupUpgradeViewModel
    {
        public int MaximumHouseholdAllowed { get; set; }
        public GroupInfo GroupInfo { get; set; }
        public SelectList GroupTypes { get; set; }
        public bool AddExisting { get; set; }
    }
}

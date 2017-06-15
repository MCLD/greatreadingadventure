using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class ItemsListViewModel
    {
        public IEnumerable<GRA.Domain.Model.DynamicAvatarItem> Items { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}

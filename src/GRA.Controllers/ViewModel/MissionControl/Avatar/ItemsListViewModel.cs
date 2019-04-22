using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class ItemsListViewModel
    {
        public IEnumerable<GRA.Domain.Model.AvatarItem> Items { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int Id { get; set; }
        public string Search { get; set; }
        public bool Available { get; set; }
        public bool Unavailable { get; set; }
        public bool Unlockable { get; set; }
        public int ItemId { get; set; }
    }
}

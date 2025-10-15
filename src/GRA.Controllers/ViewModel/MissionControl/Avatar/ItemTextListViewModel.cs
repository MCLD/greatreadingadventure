using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class ItemTextListViewModel : PaginateViewModel
    {
        public IEnumerable<AvatarItem> Items { get; set; }
        public IEnumerable<Language> Languages { get; set; }
        public IEnumerable<AvatarItemText> Texts { get; set; }
        public IEnumerable<int> DeleteIds { get; set; }
        public int? AltTextMaxLength { get; set; }
        public bool TextMissing { get; set; }
    }
}

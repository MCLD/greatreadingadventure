using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class ColorTextListViewModel : PaginateViewModel
    {
        public IEnumerable<AvatarColor> Colors { get; set; }
        public IEnumerable<Language> Languages { get; set; }
        public IEnumerable<AvatarColorText> Texts { get; set; }
        public int? AltTextMaxLength { get; set; }
        public bool TextMissing { get; set; }
    }
}

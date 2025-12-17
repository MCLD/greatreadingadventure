using System.Collections.Generic;

namespace GRA.Domain.Model.Filters
{
    public class AvatarFilter : BaseFilter
    {
        public bool Available { get; set; }
        public bool Unavailable { get; set; }
        public bool Unlockable { get; set; }
        public bool CanBeUnlocked { get; set; }
        public ICollection<int> ItemIds { get; set; }
        public int? LayerId { get; set; }
        public int? LanguageId { get; set; }
        public bool? TextMissing { get; set; }

        public AvatarFilter(int? page = null, int take = 10) : base(page, take) { }
    }
}

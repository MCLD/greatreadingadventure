using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GRA.Data.Model
{
    public class AvatarItem : Abstract.BaseDbEntity
    {
        [Required]
        public int AvatarLayerId { get; set; }
        public AvatarLayer AvatarLayer { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        public int SortOrder { get; set; }

        [MaxLength(255)]
        public string Thumbnail { get; set; }

        public bool Unlockable { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GRA.Data.Model
{
    public class DynamicAvatarItem : Abstract.BaseDbEntity
    {
        [Required]
        public int DynamicAvatarLayerId { get; set; }
        public DynamicAvatarLayer DynamicAvatarLayer { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        public int SortOrder { get; set; }

        [MaxLength(255)]
        public string Thumbnail { get; set; }

        public bool Unlockable { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Model
{
    public class Permission : Abstract.BaseDbEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}

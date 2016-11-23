using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Model
{
    public class Role : Abstract.BaseDomainEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}

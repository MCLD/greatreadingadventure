using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Branch : Abstract.BaseDomainEntity
    {
        [DisplayName("System")]
        [Required]
        public int SystemId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Url { get; set; }

        [Required]
        [MaxLength(255)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string Telephone { get; set; }

        [MaxLength(50)]
        public string Geolocation { get; set; }

        public string SystemName { get; set; }

        public ICollection<PsBranchSelection> Selections { get; set; }

        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}

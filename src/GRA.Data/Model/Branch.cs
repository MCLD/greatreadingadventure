using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Branch : Abstract.BaseDbEntity
    {
        [Required]
        public int SystemId { get; set; }

        public virtual System System { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Url { get; set; }

        [Required]
        [MaxLength(255)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string Telephone { get; set; }

        [MaxLength(50)]
        public string Geolocation { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}

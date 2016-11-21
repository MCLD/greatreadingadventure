using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Abstract
{
    public abstract class BaseDbEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}

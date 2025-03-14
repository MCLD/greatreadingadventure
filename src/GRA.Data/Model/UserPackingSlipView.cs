using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class UserPackingSlipView
    {
        [Key]
        [MaxLength(255)]
        [Required]
        public string PackingSlip { get; set; }

        public User User { get; set; }

        [Key]
        [Required]
        public int UserId { get; set; }

        public DateTime ViewedAt { get; set; }
    }
}

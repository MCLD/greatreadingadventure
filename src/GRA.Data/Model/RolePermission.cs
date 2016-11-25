using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        public int PermissionId { get; set; }
        public virtual Permission Permission { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

    }
}

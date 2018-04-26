﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AuthorizationCode : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Code { get; set; }
        public string Description { get; set; }
        [Required]
        [DisplayName("Role")]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int Uses { get; set; }
        [DisplayName("Single Use")]
        public bool IsSingleUse { get; set; }
    }
}

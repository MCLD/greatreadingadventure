using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class User : Abstract.BaseDomainEntity
    {
        [Required]
        public int SiteId { get; set; }
        [MaxLength(254)]
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLockedOut { get; set; }
        [MaxLength(36)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }
        [MaxLength(255)]
        public string LastName { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        [MaxLength(32)]
        public string PostalCode { get; set; }
        [MaxLength(64)]
        public string CardNumber { get; set; }
        public DateTime? LastAccess { get; set; }
        public int BranchId { get; set; }
        public int SystemId { get; set; }
        public int PointsEarned { get; set; }
        public int ProgramId { get; set; }
        public int AvatarId { get; set; }
        public int HeadOfHouseholdUserId { get; set; }
    }
}

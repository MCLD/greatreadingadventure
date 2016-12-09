using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class User : Abstract.BaseDomainEntity
    {
        [Required]
        public int SiteId { get; set; }
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        public bool CanBeDeleted { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime LockedOutAt { get; set; }
        public string LockedOutFor { get; set; }
        public bool IsAchiever { get; set; }
        [MaxLength(36)]
        public string Username { get; set; }

        [DisplayName("First Name")]
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        [MaxLength(255)]
        public string LastName { get; set; }
        [DisplayName("Phone Number")]
        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        [MaxLength(32)]
        public string PostalCode { get; set; }
        [MaxLength(64)]
        public string CardNumber { get; set; }
        public DateTime? LastAccess { get; set; }
        [DisplayName("Branch")]
        public int BranchId { get; set; }
        [DisplayName("System")]
        public int SystemId { get; set; }
        public int PointsEarned { get; set; }
        [DisplayName("Program")]
        public int ProgramId { get; set; }
        public string StaticAvatarFilename { get; set; }
        public int? AvatarId { get; set; }
        public int? HouseholdHeadUserId { get; set; }
        public string BranchName { get; set; }
        public string SystemName { get; set; }
        public string ProgramName { get; set; }
        public string FullName {
            get {
                return FirstName + " " + LastName;
            }
        }
    }
}

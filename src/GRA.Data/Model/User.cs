using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class User : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }
        [MaxLength(254)]
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        public bool CanBeDeleted { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime LockedOutAt { get; set; }
        public string LockedOutFor { get; set; }

        public bool IsAchiever { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastActivityDate { get; set; }

        [MaxLength(36)]
        public string Username { get; set; }
        public string PasswordHash { get; set; }

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
        public virtual Branch Branch { get; set; }
        public int SystemId { get; set; }
        public virtual System System { get; set; }
        public int PointsEarned { get; set; }
        public int ProgramId { get; set; }
        public virtual Program Program { get; set; }
        public int? AvatarId { get; set; }
        public int? HouseholdHeadUserId { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

        public int? Age { get; set; }
        public int? SchoolId { get; set; }
        public int? EnteredSchoolId { get; set; }
        public virtual EnteredSchool EnteredSchool { get; set; }

        public DateTime? AchievedAt { get; set; }
        public DateTime? LastBroadcast { get; set; }
    }
}

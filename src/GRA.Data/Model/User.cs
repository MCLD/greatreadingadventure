using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class User : Abstract.BaseDbEntity
    {
        public DateTime? AchievedAt { get; set; }

        public int? Age { get; set; }

        public virtual Branch Branch { get; set; }

        public int BranchId { get; set; }

        public bool CanBeDeleted { get; set; }

        public bool CannotBeEmailed { get; set; }

        [MaxLength(64)]
        public string CardNumber { get; set; }

        [MaxLength(8)]
        public string Culture { get; set; }

        public int? DailyPersonalGoal { get; set; }

        [MaxLength(254)]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        public int? HouseholdHeadUserId { get; set; }

        public bool IsActive { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsEmailSubscribed { get; set; }

        public bool IsFirstTime { get; set; }

        public bool IsHomeschooled { get; set; }

        public bool IsLockedOut { get; set; }

        public bool IsMcRegistered { get; set; }

        public bool IsNewsSubscribed { get; set; }

        public bool IsSystemUser { get; set; }

        public DateTime? LastAccess { get; set; }

        public DateTime? LastActivityDate { get; set; }

        public DateTime? LastBroadcast { get; set; }

        [MaxLength(255)]
        public string LastName { get; set; }

        public DateTime LockedOutAt { get; set; }

        public string LockedOutFor { get; set; }

        public string PasswordHash { get; set; }

        public int? PersonalPointGoal { get; set; }

        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        public int PointsEarned { get; set; }

        [MaxLength(32)]
        public string PostalCode { get; set; }

        public virtual Program Program { get; set; }

        public int ProgramId { get; set; }

        public int? SchoolId { get; set; }

        public bool SchoolNotListed { get; set; }

        public virtual Site Site { get; set; }

        [Required]
        public int SiteId { get; set; }

        public virtual System System { get; set; }

        public int SystemId { get; set; }

        [MaxLength(16)]
        public string UnsubscribeToken { get; set; }

        [MaxLength(36)]
        public string Username { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}

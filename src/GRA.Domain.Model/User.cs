using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class User : Abstract.BaseDomainEntity
    {
        public DateTime? AchievedAt { get; set; }

        [DisplayName(DisplayNames.Age)]
        public int? Age { get; set; }

        [DisplayName(DisplayNames.Branch)]
        [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.FieldBranch)]
        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public bool CanBeDeleted { get; set; }

        public bool CanDonateVendorCode { get; set; }

        public bool CanEmailAwardVendorCode { get; set; }

        [MaxLength(64)]
        public string CardNumber { get; set; }

        [MaxLength(8)]
        public string Culture { get; set; }

        [DisplayName(DisplayNames.DailyPersonalGoal)]
        public int? DailyPersonalGoal { get; set; }

        public bool? Donated { get; set; }

        [DisplayName(DisplayNames.EmailAddress)]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; }

        public bool? EmailAwarded { get; set; }
        public string EmailAwardInstructions { get; set; }

        [DisplayName(DisplayNames.FirstName)]
        [Required(ErrorMessage = ErrorMessages.Field)]
        [MaxLength(255)]
        public string FirstName { get; set; }

        public string FullName
        {
            get
            {
                return string.IsNullOrEmpty(LastName)
                    ? FirstName
                    : FirstName + " " + LastName;
            }
        }

        public bool HasNewMail { get; set; }
        public bool HasPendingQuestionnaire { get; set; }
        public bool HasUnclaimedPrize { get; set; }
        public int? HouseholdHeadUserId { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDonationLocked { get; set; }
        public bool IsEmailSubscribed { get; set; }

        [DisplayName(DisplayNames.IsFirstTime)]
        public bool IsFirstTime { get; set; }

        public bool IsHomeschooled { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsMcRegistered { get; set; }
        public bool IsNewsSubscribed { get; set; }

        public bool IsSystemUser { get; set; }

        public DateTime? LastAccess { get; set; }

        public DateTime? LastActivityDate { get; set; }

        public DateTime? LastBroadcast { get; set; }

        [DisplayName(DisplayNames.LastName)]
        [MaxLength(255)]
        public string LastName { get; set; }

        public DateTime LockedOutAt { get; set; }

        public string LockedOutFor { get; set; }

        public bool NeedsToAnswerVendorCodeQuestion { get; set; }

        [DisplayName(DisplayNames.PersonalPointGoal)]
        public int? PersonalPointGoal { get; set; }

        [DisplayName(DisplayNames.PhoneNumber)]
        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        public int PointsEarned { get; set; }

        [DisplayName(DisplayNames.ZipCode)]
        [MaxLength(32)]
        public string PostalCode { get; set; }

        [DisplayName(DisplayNames.Program)]
        [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.FieldProgram)]
        public int ProgramId { get; set; }

        public string ProgramName { get; set; }

        [DisplayName(DisplayNames.School)]
        public int? SchoolId { get; set; }

        public bool SchoolNotListed { get; set; }

        [Required]
        public int SiteId { get; set; }

        public string StaticAvatarFilename { get; set; }

        [DisplayName(DisplayNames.System)]
        [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.FieldSystem)]
        public int SystemId { get; set; }

        public string SystemName { get; set; }

        [MaxLength(16)]
        public string UnsubscribeToken { get; set; }

        [DisplayName(DisplayNames.Username)]
        [MaxLength(36)]
        public string Username { get; set; }

        public string VendorCode { get; set; }
        public string VendorCodeMessage { get; set; }
        public string VendorCodePackingSlip { get; set; }
        public string VendorCodeUrl { get; set; }
        public bool VendorIsDamaged { get; set; }
        public bool VendorIsMissing { get; set; }
        public VendorOrderStatus? VendorOrderStatus { get; set; }
    }
}

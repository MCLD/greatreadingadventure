using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class PsPerformer : Abstract.BaseDbEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        [DisplayName("Presenter/Organization Name")]
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        [MaxLength(255)]
        [Required]
        public string ContactName { get; set; }

        [MaxLength(500)]
        [Required]
        public string BillingAddress { get; set; }

        [MaxLength(255)]
        public string Website { get; set; }

        [MaxLength(255)]
        [Required]
        public string Email { get; set; }

        [MaxLength(255)]
        [Required]
        public string Phone { get; set; }

        public bool PhonePreferred { get; set; }

        [MaxLength(255)]
        [Required]
        public string VendorId { get; set; }

        [DisplayName("I have a fingerprint clearance card")]
        public bool HasFingerprintCard { get; set; }

        [DisplayName("I have liability insurance")]
        public bool HasInsurance { get; set; }

        public bool AllBranches { get; set; }

        [MaxLength(2000)]
        [Required]
        public string References { get; set; }

        public bool SetSchedule { get; set; }
        public bool RegistrationCompleted { get; set; }
        public bool IsApproved { get; set; }
    }
}

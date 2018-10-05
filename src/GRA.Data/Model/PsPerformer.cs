using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class PsPerformer : Abstract.BaseDbEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

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

        public bool AllBranches { get; set; }

        [MaxLength(255)]
        public string ReferencesFilename { get; set; }

        public bool SetSchedule { get; set; }
        public bool RegistrationCompleted { get; set; }
        public bool IsApproved { get; set; }

        public ICollection<PsPerformerBranch> Branches { get; set; }
        public IList<PsPerformerImage> Images { get; set; }
        public IList<PsPerformerSchedule> Schedule { get; set; }

        public ICollection<PsProgram> Programs { get; set; }
    }
}

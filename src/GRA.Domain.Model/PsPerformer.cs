using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class PsPerformer : Abstract.BaseDomainEntity
    {
        public int UserId { get; set; }

        [DisplayName("Presenter/Organization name")]
        [MaxLength(255, ErrorMessage = "Please enter no more than {1} characters for {0}.")]
        [Required]
        public string Name { get; set; }

        [DisplayName("Contact name")]
        [MaxLength(255, ErrorMessage = "Please enter no more than {1} characters for {0}.")]
        [Required]
        public string ContactName { get; set; }

        [DisplayName("Billing address")]
        [MaxLength(500, ErrorMessage = "Please enter no more than {1} characters for {0}.")]
        [Required]
        public string BillingAddress { get; set; }

        [DisplayName("Web site")]
        [MaxLength(255, ErrorMessage = "Please enter no more than {1} characters for {0}.")]
        public string Website { get; set; }

        [MaxLength(255, ErrorMessage = "Please enter no more than {1} characters for {0}.")]
        [Required]
        public string Email { get; set; }

        [MaxLength(255, ErrorMessage = "Please enter no more than {1} characters for {0}.")]
        [Required]
        public string Phone { get; set; }

        [DisplayName("Preferred method of contact")]
        public bool PhonePreferred { get; set; }

        [DisplayName("Vendor ID number")]
        [MaxLength(255, ErrorMessage = "Please enter no more than {1} characters for {0}.")]
        [Required]
        public string VendorId { get; set; }

        [DisplayName("I have a fingerprint clearance card")]
        public bool HasFingerprintCard { get; set; }

        [DisplayName("I have liability insurance")]
        public bool HasInsurance { get; set; }

        public bool AllBranches { get; set; }

        [MaxLength(2000, ErrorMessage = "Please enter no more than {1} characters for {0}.")]
        public string References { get; set; }

        public bool SetSchedule { get; set; }
        public bool RegistrationCompleted { get; set; }
        public bool IsApproved { get; set; }

        public ICollection<Branch> Branches { get; set; }
        public List<PsPerformerImage> Images { get; set; }
        public List<PsPerformerSchedule> Schedule { get; set; }

        public ICollection<PsProgram> Programs { get; set; }

        public bool AvailableInSystem { get; set; }
        public int ProgramCount { get; set; }
        public int SelectionsCount { get; set; }
    }
}

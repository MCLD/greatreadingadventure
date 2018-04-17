using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class PointTranslation : Abstract.BaseDomainEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        [MaxLength(255)]
        [DisplayName("Name")]
        public string TranslationName { get; set; }
        [Required]
        [MaxLength(255)]
        [DisplayName("Activity Description")]
        public string ActivityDescription { get; set; }
        [Required]
        [MaxLength(255)]
        [DisplayName("Activity Description Plural")]
        public string ActivityDescriptionPlural { get; set; }
        [Required]
        [DisplayName("Activity Amount")]
        public int ActivityAmount { get; set; }
        [Required]
        [DisplayName("Points Earned")]
        public int PointsEarned { get; set; }
        [Required]
        [DisplayName("Single Event")]
        public bool IsSingleEvent { get; set; }
        [Required]
        [DisplayName("Translation Description Present Tense")]
        public string TranslationDescriptionPresentTense { get; set; }
        [Required]
        [DisplayName("Translation Description Past Tense")]
        public string TranslationDescriptionPastTense { get; set; }
    }
}

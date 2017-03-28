using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class RequiredQuestionnaire : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        [Required]
        public int QuestionnaireId { get; set; }
        public int? AgeMaximum { get; set; }
        public int? AgeMinimum { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

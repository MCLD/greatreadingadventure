using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Mail : Abstract.BaseDomainEntity
    {
        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.Body)]
        [MaxLength(2000)]
        public string Body { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        public bool CanParticipantDelete { get; set; }

        public int? DrawingId { get; set; }

        [Required]
        public int FromUserId { get; set; }

        public int? InReplyToId { get; set; }
        public bool IsBroadcast { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        public bool IsNew { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        public bool IsRepliedTo { get; set; }

        public int SiteId { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.Subject)]
        [MaxLength(500)]
        public string Subject { get; set; }

        public IDictionary<string, string> TemplateDictionary { get; set; }
        public int? ThreadId { get; set; }
        public int? ToUserId { get; set; }
        public int? TriggerId { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Mail
{
    public class MailCreateViewModel
    {
        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.Subject)]
        [MaxLength(500)]
        public string Subject { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.Body)]
        [MaxLength(2000)]
        public string Body { get; set; }

        public int? InReplyToId { get; set; }

        [DisplayName(Annotations.Interface.ReplyTo)]
        public string InReplyToSubject { get; set; }
    }
}

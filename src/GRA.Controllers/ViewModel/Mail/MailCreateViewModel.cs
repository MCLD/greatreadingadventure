using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.Mail
{
    public class MailCreateViewModel
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
        public int? InReplyToId { get; set; }
        public string InReplyToSubject { get; set; }
    }
}

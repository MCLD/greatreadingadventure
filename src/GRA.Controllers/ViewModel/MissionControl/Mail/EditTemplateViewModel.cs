using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.MissionControl.Mail
{
    public class EditTemplateViewModel
    {
        public string Body { get; set; }
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public string ReturnLink { get; set; }

        [MaxLength(255)]
        public string Subject { get; set; }
    }
}

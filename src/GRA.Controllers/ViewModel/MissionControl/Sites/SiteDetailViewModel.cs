using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Sites
{
    public class SiteDetailViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Path { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [DisplayName("Page Title")]
        [Required]
        [MaxLength(255)]
        public string PageTitle { get; set; }
        public string Footer { get; set; }
        [DisplayName("Meta Description")]
        [MaxLength(150)]
        public string MetaDescription { get; set; }
    }
}

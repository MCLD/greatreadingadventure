using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Page : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        [Required]
        [MaxLength(255)]
        public string Stub { get; set; }
        [Required]
        public string Content { get; set; }
        [DisplayName("Footer Text")]
        [MaxLength(255)]
        public string FooterText { get; set; }
        [DisplayName("Nav Text")]
        [MaxLength(255)]
        public string NavText { get; set; }
        [DisplayName("Publish this page")]
        public bool IsPublished { get; set; }
    }
}
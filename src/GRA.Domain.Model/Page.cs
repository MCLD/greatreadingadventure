using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Page : Abstract.BaseDomainEntity
    {
        [Required]
        public string Content { get; set; }

        [DisplayName("Footer link to this page (leave blank for no footer link)")]
        [MaxLength(255)]
        public string FooterText { get; set; }

        [DisplayName("Publish this page")]
        public bool IsPublished { get; set; }

        [Required]
        public int LanguageId { get; set; }

        [DisplayName("Meta page description for search engines")]
        [MaxLength(150)]
        public string MetaDescription { get; set; }

        [DisplayName("Top navigation link to this page (leave blank for no top link)")]
        [MaxLength(255)]
        public string NavText { get; set; }

        [Required]
        public int PageHeaderId { get; set; }

        public string PageStub { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
    }
}

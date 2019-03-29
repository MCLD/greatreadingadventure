using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Page : Abstract.BaseDomainEntity
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

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

        [Required]
        public int LanguageId { get; set; }

        [Required]
        public int PageHeaderId { get; set; }

        [MaxLength(150)]
        public string MetaDescription { get; set; }

        public string PageStub { get; set; }
    }
}
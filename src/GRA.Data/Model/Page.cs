using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Page : Abstract.BaseDbEntity
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [MaxLength(255)]
        public string FooterText { get; set; }

        [MaxLength(255)]
        public string NavText { get; set; }

        public bool IsPublished { get; set; }

        [Required]
        public int LanguageId { get; set; }

        [Required]
        public int PageHeaderId { get; set; }

        [MaxLength(150)]
        public string MetaDescription { get; set; }
    }
}
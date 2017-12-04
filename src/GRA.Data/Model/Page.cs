using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Page : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        [MaxLength(255)]
        public string Stub { get; set; }
        [Required]
        public string Content { get; set; }
        [MaxLength(255)]
        public string FooterText { get; set; }
        [MaxLength(255)]
        public string NavText { get; set; }
        public bool IsPublished { get; set; }
    }
}
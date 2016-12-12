using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Page : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string Stub { get; set; }
        [Required]
        public string Content { get; set; }
        public bool IsPublished { get; set; }
    }
}
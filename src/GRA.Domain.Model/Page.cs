using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Page : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Stub { get; set; }
        [Required]
        public string Content { get; set; }
        public bool IsPublished { get; set; }
    }
}
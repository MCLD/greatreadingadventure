using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Mail : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        public int ToUserId { get; set; }
        [Required]
        public int FromUserId { get; set; }

        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public bool IsNew { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
    }
}

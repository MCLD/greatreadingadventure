using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class System
    {
        public int Id { get; set; }
        [Required]
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}

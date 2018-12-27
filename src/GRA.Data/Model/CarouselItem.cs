using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class CarouselItem : Abstract.BaseDbEntity
    {
        [Required]
        public int CarouselId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; }

        [Required]
        public int SortOrder { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
    }
}

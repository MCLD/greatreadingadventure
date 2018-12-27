using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class CarouselItem : Abstract.BaseDomainEntity
    {
        [Required]
        public int CarouselId { get; set; }

        [Required]
        [Display(Description = "displayed in the pop-up when the item is clicked")]
        public string Description { get; set; }

        [Required]
        [MaxLength(500)]
        [Display(Name = "Image URL",
            Description = "link to an image, please ensure you are allowed to directly link!")]
        public string ImageUrl { get; set; }

        [Required]
        public int SortOrder { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Description = "displayed under the item in the carousel")]
        public string Title { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Book : Abstract.BaseDomainEntity
    {
        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.Title)]
        [MaxLength(500)]
        public string Title { get; set; }

        [DisplayName(DisplayNames.Author)]
        [MaxLength(255)]
        public string Author { get; set; }

        [MaxLength(30)]
        public string Isbn { get; set; }

        [MaxLength(500)]
        public string Url { get; set; }

        public int? ChallengeId { get; set; }
    }
}

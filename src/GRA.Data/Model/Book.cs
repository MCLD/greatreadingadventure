using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Book : Abstract.BaseDbEntity
    {
        [Required]
        [MaxLength(500)]
        public string Title { get; set; }
        [MaxLength(255)]
        public string Author { get; set; }
        [MaxLength(30)]
        public string Isbn { get; set; }

        [MaxLength(500)]
        public string Url { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Site
    {
        [Key]
        public string Path { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
    }
}

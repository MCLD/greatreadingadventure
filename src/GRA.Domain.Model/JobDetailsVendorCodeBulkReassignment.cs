using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class JobDetailsVendorCodeBulkReassignment
    {
        public string Filename { get; set; }

        [MaxLength(255)]
        public string Reason { get; set; }
    }
}

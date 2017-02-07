using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class SchoolDetails
    {
        public ICollection<School> Schools { get; set; }
        public int SchoolDisctrictId { get; set; }
        public int SchoolTypeId { get; set; }
    }
}

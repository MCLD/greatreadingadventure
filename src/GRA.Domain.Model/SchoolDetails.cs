using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class SchoolDetails
    {
        public School School { get; set; }
        public ICollection<School> Schools { get; set; }
        public int SchoolDistrictId { get; set; }
        public int? SchoolTypeId { get; set; }
    }
}

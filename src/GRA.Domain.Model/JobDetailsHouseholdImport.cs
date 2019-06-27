using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Domain.Model
{
    public class JobDetailsHouseholdImport
    {
        public string Filename { get; set; }
        public int HeadOfHouseholdId { get; set; }
        public int ProgmamId { get; set; }
        public int? SchoolId { get; set; }
        public bool IsHomeSchooled { get; set; }
        public bool SchoolNotListed { get; set; }
        public bool FirstTimeParticipating { get; set; }
    }
}

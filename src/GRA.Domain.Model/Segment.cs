using System;
using GRA.Domain.Model.Abstract;

namespace GRA.Domain.Model
{
    public class Segment : BaseDomainEntity
    {
        public DateTime? EndDate { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class PsBlackoutDate : Abstract.BaseDomainEntity
    {
        public DateTime Date { get; set; }
        [MaxLength(255)]
        public string Reason { get; set; }
    }
}

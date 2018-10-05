using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class PsBlackoutDate : Abstract.BaseDbEntity
    {
        public DateTime Date { get; set; }
        [MaxLength(255)]
        public string Reason { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class PsScheduleDate
    {
        public string Availability { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Date { get; set; }
        public List<string> Time { get; set; }

        public DateTime ParsedDate { get; set; }
        public PsScheduleDateStatus Status { get; set; }
    }


    public enum PsScheduleDateStatus
    {
        Available,
        Current,
        Time,
        Unavailable
    }
}

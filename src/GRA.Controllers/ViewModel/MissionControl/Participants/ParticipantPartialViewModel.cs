using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public abstract class ParticipantPartialViewModel
    {
        public int Id { get; set; }
        public int HouseholdCount { get; set; }
        public int PrizeCount { get; set; }
        public bool HasAccount { get; set; }
        public bool IsGroup { get; set; }
    }
}

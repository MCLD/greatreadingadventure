using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class LogActivityViewModel
    {
        public int Id { get; set; }
        public int HouseholdCount { get; set; }
        public bool HasAccount { get; set; }
        [DisplayName("Minutes Read")]
        public int? MinutesRead { get; set; }
        [DisplayName("Secret Code")]
        public string SecretCode { get; set; }
        public bool IsSecretCode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.Events
{
    public class EventsDetailViewModel
    {
        public GRA.Domain.Model.Event Event { get; set; }
        public string ProgramString { get; set; }
    }
}

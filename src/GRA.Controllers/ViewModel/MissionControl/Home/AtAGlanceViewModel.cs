using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Home
{
    public class AtAGlanceViewModel
    {
        public IEnumerable<object> NewsPosts { get; set; }
        public AtAGlanceReport AtAGlanceReport { get; set; }
    }
}

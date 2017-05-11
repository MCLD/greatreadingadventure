using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Controllers.ViewModel.MissionControl.Drawing
{
    public class DrawingNewViewModel
    {
        public GRA.Domain.Model.Drawing Drawing { get; set; }
        public bool CanSendMail { get; set; }
    }
}

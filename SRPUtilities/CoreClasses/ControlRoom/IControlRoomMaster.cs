using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using STG.SRP.ControlRoom;

namespace STG.SRP.ControlRooms
{
    public interface IControlRoomMaster 
    {
        STG.SRP.ControlRoom.CRRibbon PageRibbon { get; }

        string  PageError { set; }
        string PageWarning { set; }
        string PageMessage { set; }

        string PageTitle { get; set; }
        bool IsSecure { get; set; }
        long RequiredPermission { get; set; }
        bool DisplayMessageOnLoad { get; set; }
    }
}
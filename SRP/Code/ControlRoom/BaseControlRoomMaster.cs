using GRA.SRP.ControlRoom;
using System;
using System.Web.UI;

namespace SRPApp.Classes {
    public class BaseControlRoomMaster : MasterPage {
        #region Properties
        protected BaseControlRoomPage CRPage;
        #endregion

        protected void PageLoad(object sender, EventArgs e) {
            try { CRPage = (BaseControlRoomPage)this.Page; } catch { }
        }
    }
}

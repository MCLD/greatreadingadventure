
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;

namespace GRA.SRP.ControlRoom.Modules.Patrons
{
    public class PatronsRibbon
    {
        public static void GetByAppContext(BaseControlRoomPage p)
        {

                p.SetPageRibbon(StandardModuleRibbons.PatronRibbon());

                if (HttpContext.Current.Session["Curr_Patron"] != null)
                {
                    var cp = (Patron)HttpContext.Current.Session["Curr_Patron"];
                    var r = StandardModuleRibbons.SelectedPatronRibbon();
                    var pnl = ((RibbonPanel)r[0]);
                    if (cp.IsMasterAccount)
                        pnl.Add(new RibbonLink { Name = "<font color=Red >Sub Account List</font>", Url = "/ControlRoom/Modules/Patrons/PatronsSubAccounts.aspx" });
                    if (cp.MasterAcctPID != 0)
                        pnl.Add(new RibbonLink { Name = "<font color=Green >Switch To Master Account</font>", Url = "/ControlRoom/Modules/Patrons/PatronsMasterAccount.aspx" });
                    pnl.Add(new RibbonLink { Name = "Add Sub Account", Url = "/ControlRoom/Modules/Patrons/PatronsAddSubAccount.aspx" });
                    p.SetPageRibbon(r);

                    p.SetPageRibbon(StandardModuleRibbons.SelectedPatronOtherRibbon());
                }
            
        }

        
    }
}
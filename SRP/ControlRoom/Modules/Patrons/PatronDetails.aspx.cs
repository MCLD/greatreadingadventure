using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;

namespace GRA.SRP.ControlRoom.Modules.Patrons
{
    public partial class PatronDetails : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5100;
            MasterPage.IsSecure = true;

            string currentPatronMode = Session["CURR_PATRON_MODE"] as string;

            if(!string.IsNullOrEmpty(currentPatronMode))
            {
                switch(currentPatronMode)
                {
                    case "ADDSUB":
                        MasterPage.PageTitle = "Add Patron SUB Account";
                        break;
                    case "ADDNEW":
                        MasterPage.PageTitle = "Add Patron Account";
                        break;
                    case "EDIT":
                        MasterPage.PageTitle = "Patron Account Details";
                        break;

                }
            }

            if (!IsPostBack)
            {
                PatronsRibbon.GetByAppContext(this);
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["pid"] != null)
                {
                    int pid = -1;
                    if (int.TryParse(Request.QueryString["pid"].ToString(), out pid))
                    {
                        Session["CURR_PATRON_ID"] = pid;
                        Session["CURR_PATRON_MODE"] = "EDIT";
                        var p = Patron.FetchObject(pid);
                        Session["CURR_PATRON"] = p;
                        Response.Redirect(Request.Url.AbsolutePath);
                    }
                }
                if (Session["Curr_Patron"] == null)
                {
                    PatronCtl1.PatronID = string.Empty;
                    PatronCtl1.MasterPatronID = string.Empty;
                    PatronCtl1.LoadControl();
                }
                else
                {
                    PatronCtl1.PatronID = Session["CURR_PATRON_ID"].ToString();
                    if (Session["CURR_PATRON_MODE"].ToString() == "EDIT")
                    {
                        var p = Patron.FetchObject(int.Parse(Session["CURR_PATRON_ID"].ToString()));
                        if (p.MasterAcctPID != 0)
                        {
                            PatronCtl1.MasterPatronID = p.MasterAcctPID.ToString();
                        }
                        else
                        {
                            PatronCtl1.MasterPatronID = string.Empty;
                        }
                    }

                    PatronCtl1.LoadControl();
                }
            }
            //Session["CURR_PATRON_ID"] = key;
            //Session["CURR_PATRON"] = Patron.FetchObject(key);
        }
    }
}
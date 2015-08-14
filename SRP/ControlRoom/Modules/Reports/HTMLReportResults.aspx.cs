using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;


namespace GRA.SRP.ControlRoom.Modules.Reports
{
    public partial class HTMLReportResults : BaseControlRoomPage
    {
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Ad-Hoc Report Results");

            if (!IsPostBack)
            {
                if (Session["rptFilter"] != null && Session["rptSql"] != null) 
                {
                    lblFilter.Text = (string) Session["rptFilter"];
                    var ds = (DataSet)Session["rptSql"];
                    gv.DataSource = ds;
                    gv.DataBind();
                    //Session["rptFilter"] = null;
                    //Session["rptSql"] = null;
                }
                else
                {
                    lblFilter.Text = "Invalid report results;";
                }
                Page.DataBind();
            }

        }
    }
}
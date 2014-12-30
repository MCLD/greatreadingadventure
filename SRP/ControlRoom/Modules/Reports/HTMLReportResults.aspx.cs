﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using SRPApp.Classes;
using STG.SRP.ControlRooms;
using STG.SRP.Core.Utilities;
using STG.SRP.DAL;
using STG.SRP.Utilities.CoreClasses;


namespace STG.SRP.ControlRoom.Modules.Reports
{
    public partial class HTMLReportResults : BaseControlRoomPage
    {
        private static string conn = STG.SRP.Core.Utilities.GlobalUtilities.SRPDB;


        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    SetPageRibbon(StandardModuleRibbons.ReportsRibbon());
            //}

            //MasterPage.RequiredPermission = PERMISSIONID;
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
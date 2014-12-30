﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using SRPApp.Classes;
using STG.SRP.ControlRooms;
using STG.SRP.Controls;
using STG.SRP.Core.Utilities;
using STG.SRP.DAL;

namespace STG.SRP.ControlRoom.Modules.Setup
{
    public partial class AwardManual : BaseControlRoomPage
    {
        private static string conn = STG.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MasterPage.IsSecure = true;
                MasterPage.PageTitle = string.Format("{0}", "Manual Bulk Badge Awards");

                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            var ds = GetMatchingData();
            lblCount.Text = string.Format("There are {0:#,##0} patrons matching your criteria.  {1}", ds.Tables[0].Rows.Count, (ds.Tables[0].Rows.Count>1000 ? " <b>Be advised, this may take a while..." : ""));
            pnlResults.Visible = true;
        }

        private DataSet GetMatchingData()
        {
            var arrParams = new SqlParameter[4];

            if (ProgID.SelectedValue == "0")
            {
                arrParams[0] = new SqlParameter("@ProgId", DBNull.Value);
            }
            else
            {
                arrParams[0] = new SqlParameter("@ProgId", ProgID.SelectedValue);
            }
            if (BranchID.SelectedValue == "0")
            {
                arrParams[1] = new SqlParameter("@BranchID", DBNull.Value);
            }
            else
            {
                arrParams[1] = new SqlParameter("@BranchID", BranchID.SelectedValue);
            }
            if (LibSys.SelectedValue == "")
            {
                arrParams[2] = new SqlParameter("@LibSys", DBNull.Value);
            }
            else
            {
                arrParams[2] = new SqlParameter("@LibSys", LibSys.SelectedValue);
            }
            if (School.SelectedValue == "")
            {
                arrParams[3] = new SqlParameter("@School", DBNull.Value);
            }
            else
            {
                arrParams[3] = new SqlParameter("@School", School.SelectedValue);
            }

            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "rpt_PatronFilter", arrParams);

            return ds;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ProgID.SelectedValue = BranchID.SelectedValue = "0";
            School.SelectedValue = LibSys.SelectedValue = "";
            BadgeID.SelectedValue = "0";
            pnlResults.Visible = false;
        }

        protected void btnAward_Click(object sender, EventArgs e)
        {
            if (BadgeID.SelectedValue == "0")
            {
                var masterPage = (IControlRoomMaster)Master;
                if (masterPage != null) masterPage.PageError = "You need to select a badge.";
                lblAwards.Text = "A total of 0 badges was awarded";
                return;
            }
            var bid = Convert.ToInt32(BadgeID.SelectedValue);
            var awardCount = 0;
            foreach (DataRow row in GetMatchingData().Tables[0].Rows)
            {
                var pid = Convert.ToInt32(row["PID"]);
                var list = new List<Badge>();
                var p = Patron.FetchObject(pid);
                if (AwardPoints.AwardBadgeToPatron(bid, p, ref list))
                {
                    awardCount++;
                    // if the y got a badge, then maybe they match the criteria to get another as well ...
                    AwardPoints.AwardBadgeToPatronViaMatchingAwards(p, ref list);
                } 
            }
            lblAwards.Text = string.Format("A total of {0:#,##0} badges was awarded.  (May be less than patrons matching criteria because the badge was not awarded to patrons already owning that badge", awardCount);
        }

    }
}
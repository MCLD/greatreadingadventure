using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Controls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class AwardManual : BaseControlRoomPage
    {
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4900;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Manual Bulk Badge Awards");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            var ds = GetMatchingData();
            lblCount.Text = string.Format("There are {0:#,##0} patrons matching your criteria.  {1}", ds.Tables[0].Rows.Count, (ds.Tables[0].Rows.Count > 1000 ? " <b>Be advised, this may take a while..." : ""));
            pnlResults.Visible = true;
        }

        private DataSet GetMatchingData()
        {
            var arrParams = new SqlParameter[9];

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
            arrParams[4] = new SqlParameter("@TenID", CRTenantID == null ? -1 : CRTenantID);

            if (NumPoints.Text == "")
            {
                arrParams[5] = new SqlParameter("@Points", 0);
            }
            else
            {
                arrParams[5] = new SqlParameter("@Points", NumPoints.Text.SafeToInt());
            }
            arrParams[6] = new SqlParameter("@PointType", DDPointAwardReason.SelectedValue.SafeToInt());

            if (StartDate.Text == "")
            {
                arrParams[7] = new SqlParameter("@StartDate", DBNull.Value);
            }
            else
            {
                arrParams[7] = new SqlParameter("@StartDate", StartDate.Text.SafeToDateTime());
            }

            if (EndDate.Text == "")
            {
                arrParams[8] = new SqlParameter("@EndDate", DBNull.Value);
            }
            else
            {
                arrParams[8] = new SqlParameter("@EndDate", EndDate.Text.SafeToDateTime());
            }

            //var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "rpt_PatronFilter", arrParams);
            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "rpt_PatronFilter_Expanded", arrParams);
            return ds;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ProgID.SelectedValue = BranchID.SelectedValue = "0";
            School.SelectedValue = LibSys.SelectedValue= string.Empty;
            BadgeID.SelectedValue = "0";
            NumPoints.Text= string.Empty;
            StartDate.Text = EndDate.Text= string.Empty;
            DDPointAwardReason.SelectedValue = "-1";
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
            Response.Write("<!-- ");
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
                    Response.Write(" :-> "); Response.Flush();
                }
            }
            Response.Write("-->");
            lblAwards.Text = string.Format("A total of {0:#,##0} badges was awarded.  (May be less than patrons matching criteria because the badge was not awarded to patrons already owning that badge", awardCount);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using SRPApp.Classes;
using STG.SRP.ControlRoom.Modules.Patrons;
using STG.SRP.ControlRooms;
using STG.SRP.Controls;
using STG.SRP.Core.Utilities;
using STG.SRP.DAL;


namespace STG.SRP.ControlRoom.Modules.Notifications
{
    public partial class BulkNotification : BaseControlRoomPage
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5000;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Bulk Notification");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.NotificationsRibbon());
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
            var arrParams = new SqlParameter[5];

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
            arrParams[4] = new SqlParameter("@TenID",
                                (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                        -1 :
                                        (int)HttpContext.Current.Session["TenantID"])
                            );
            var ds = SqlHelper.ExecuteDataset(DbConn, CommandType.StoredProcedure, "rpt_PatronFilter", arrParams);

            return ds;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ProgID.SelectedValue = BranchID.SelectedValue = "0";
            School.SelectedValue = LibSys.SelectedValue = "";
            txtSubject.Text = Body.Text = lblSent.Text = "";
            pnlResults.Visible = false;
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            var messageCount = 0;
            Response.Write("<!-- ");
            foreach (DataRow row in GetMatchingData().Tables[0].Rows)
            {
                var pid = Convert.ToInt32(row["PID"]);
                var obj = new DAL.Notifications();
                obj.PID_To = pid;
                obj.PID_From = 0;
                obj.isQuestion = false;
                obj.Subject = txtSubject.Text;
                obj.Body = Body.Text;
                obj.isUnread = true;

                obj.AddedDate = DateTime.Now;
                obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                obj.LastModDate = obj.AddedDate;
                obj.LastModUser = obj.AddedUser;

                if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                {
                    obj.Insert();
                    messageCount++;
                    Response.Write(" :-> "); Response.Flush();
                }
            }
            Response.Write("-->");
            lblSent.Text = string.Format("<br/><br/>A total of {0:#,##0} message were sent.", messageCount);
        }
    }
}
using ExportToExcel;
using GRA.SRP.Core.Utilities;
using Microsoft.ApplicationBlocks.Data;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.ControlRoom.Modules.Reports
{
    public partial class ProgramByBranch : BaseControlRoomPage
    {
        // reluctantly following the reporting pattern from the rest of the CR
        // TODO move all reporting database stuff to the DAL!
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;
        private const string ReportName = "Program By Branch";
        private const string AltReportName = "Program By Branch Pre-logging";

        private const string ReportStoredProcedure = "rpt_ProgramByBranch";
        private const string AltReportStoredProcedure = "rpt_ProgramByBranchPreLogging";

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4200;
            MasterPage.IsSecure = true;
            AlertPanel.Visible = false;

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.ReportsRibbon());
            }

            string reportName = ReportName;
            string reportSproc = ReportStoredProcedure;

            string tenantName = "Summary";

            var tenant = GRA.SRP.Core.Utilities.Tenant.FetchObject((int)CRTenantID);
            if (tenant != null)
            {
                tenantName = tenant.LandingName;
            }

            if (!string.IsNullOrEmpty(Request.QueryString["prelogging"]))
            {
                reportName = AltReportName;
                reportSproc = AltReportStoredProcedure;
            }

            var ds = SqlHelper.ExecuteDataset(conn,
                CommandType.StoredProcedure,
                reportSproc,
                new SqlParameter[]
                    {
                        new SqlParameter("@TenID", CRTenantID == null ? -1 : CRTenantID)
                    }
            );

            // fix table names
            if (ds.Tables != null && ds.Tables.Count >= 1 && ds.Tables[0].Rows.Count != 0)
            {
                ds.Tables[0].TableName = tenantName;
                int count = 1;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (ds.Tables.Count > count - 1 && ds.Tables[count] != null)
                    {
                        ds.Tables[count].TableName = row["Program"].ToString();
                    }
                    count++;
                }

                // add total rows
                count = 0;
                foreach (DataTable table in ds.Tables)
                {
                    var signupTotal = table.Compute("Sum(Signups)", "");
                    var achieverTotal = table.Compute("Sum(Achievers)", "");
                    var totalRow = table.NewRow();
                    int field = 0;
                    totalRow[field] = "Total";
                    if (count > 0)
                    {
                        field++;
                    }
                    totalRow[++field] = signupTotal;
                    totalRow[++field] = achieverTotal;
                    table.Rows.Add(totalRow);

                    string completionRate = string.Empty;
                    if ((long)signupTotal > 0)
                    {
                        completionRate = string.Format("{0:N2}%",
                            Convert.ToDouble(achieverTotal) * 100 / Convert.ToDouble(signupTotal));
                    }
                    var completionRow = table.NewRow();
                    completionRow[0] = string.Format("Completion rate: {0}", completionRate);
                    table.Rows.Add(completionRow);

                    count++;
                }

                CreateExcelFile.CreateExcelDocument(
                    ds,
                    string.Format("{0}-{1}.xlsx",
                        DateTime.Now.ToString("yyyyMMdd"),
                        reportName.Replace(" ", string.Empty)),
                    Response);
            }
            else
            {
                AlertPanel.Visible = true;
                AlertMessage.Text = "Could not find any report data.";
            }
        }
    }
}
using ExportToExcel;
using GRA.SRP.Core.Utilities;
using Microsoft.ApplicationBlocks.Data;
using SRPApp.Classes;
using System;
using System.Data.SqlClient;

namespace GRA.SRP.ControlRoom.Modules.Reports
{
    public partial class EventIssues : BaseControlRoomPage
    {
        // reluctantly following the reporting pattern from the rest of the CR
        // TODO move all reporting database stuff to the DAL!
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;
        private const string ReportName = "Event Issues";
        private const string ReportStoredProcedure = "rpt_EventIssues";

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.PageTitle = string.Format("{0} Report", ReportName);
            MasterPage.RequiredPermission = 4200;
            MasterPage.IsSecure = true;
            AlertPanel.Visible = false;

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.ReportsRibbon());
                ReportPanel.Visible = false;
            }
            else
            {
                ReportPanel.Visible = EventRepeater.Items.Count > 0;
            }
        }

        private System.Data.DataSet PrepareReport()
        {
            var eventData = SqlHelper.ExecuteDataset(conn,
                System.Data.CommandType.StoredProcedure,
                ReportStoredProcedure,
                new SqlParameter[] { new SqlParameter {
                 ParameterName = "TenID",
                 Value = CRTenantID == null ? -1 : CRTenantID
                } });

            if (eventData != null
                && eventData.Tables.Count > 0
                && eventData.Tables[0].Rows.Count > 0)
            {
                return eventData;
            }
            else
            {
                throw new Exception("Could not find any report data.");
            }

        }

        protected void ShowReport_Click(object sender, EventArgs e)
        {
            ReportPanel.Visible = true;
            AlertPanel.Visible = false;

            try
            {
                var dataSource = PrepareReport();
                EventRepeater.DataSource = dataSource;
                EventRepeater.DataBind();
            }
            catch (Exception ex)
            {
                ReportPanel.Visible = false;
                AlertPanel.Visible = true;
                AlertMessage.Text = ex.Message;
            }
        }

        protected void DownloadReport_Click(object sender, EventArgs e)
        {
            ReportPanel.Visible = true;
            AlertPanel.Visible = false;

            try
            {
                var ds = PrepareReport();
                if (ds.Tables != null && ds.Tables.Count >= 1 && ds.Tables[0].Rows.Count != 0)
                {
                    ds.Tables[0].TableName = ReportName;

                    CreateExcelFile.CreateExcelDocument(
                        ds,
                        string.Format("{0}.xlsx", ReportName.Replace(" ", string.Empty)),
                        Response);
                }
            }
            catch (Exception ex)
            {
                ReportPanel.Visible = false;
                AlertPanel.Visible = true;
                AlertMessage.Text = ex.Message;
            }
        }
    }
}

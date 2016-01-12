using System;
using System.Data;
using ExportToExcel;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;
using System.Data.SqlClient;
using System.IO;
using Microsoft.ApplicationBlocks.Data;

namespace GRA.SRP.ControlRoom.Modules.Reports
{
    public partial class MiniGameStats : BaseControlRoomPage
    {
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4200;
            MasterPage.IsSecure = true; 
            if (!IsPostBack)
            {
                MasterPage.PageTitle = string.Format("{0}", "MiniGame Play Statistics Report");

                SetPageRibbon(StandardModuleRibbons.ReportsRibbon());

            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            
            rptr.DataSource = GetReportData();;
            rptr.DataBind();

        }

        private DataSet GetReportData()
        {
            // need to add the parameters ...

            var arrParams = new SqlParameter[4];

            if (ProgID.SelectedValue == "0")
            {
                arrParams[0] = new SqlParameter("@MGID", DBNull.Value);
            }
            else
            {
                arrParams[0] = new SqlParameter("@MGID", ProgID.SelectedValue);
            }
            if (StartDate.Text == "")
            {
                arrParams[1] = new SqlParameter("@start", DBNull.Value);
            }
            else
            {
                arrParams[1] = new SqlParameter("@start", GlobalUtilities.DBSafeDate(StartDate.Text));
            }
            if (EndDate.Text == "")
            {
                arrParams[2] = new SqlParameter("@end", DBNull.Value);
            }
            else
            {
                arrParams[2] = new SqlParameter("@end", GlobalUtilities.DBSafeDate(EndDate.Text));
            }
            arrParams[3] = new SqlParameter("@TenID", CRTenantID == null ? -1 : CRTenantID);


            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "rpt_MiniGameStats", arrParams);

            return ds;
        }


        protected void btnClear_Click(object sender, EventArgs e)
        {
            ProgID.SelectedValue = "0";
            StartDate.Text = EndDate.Text= string.Empty;
            rptr.DataSource = null;
            rptr.DataBind();
        }

        protected void btnClear0_Click(object sender, EventArgs e)
        {
            var ds = GetReportData();

            string excelFilename = Server.MapPath("~/MiniGameStatsReportResults.xlsx");
            CreateExcelFile.CreateExcelDocument(ds, excelFilename);

            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("Content-Disposition", "attachment; filename=MiniGameStatsReportResults.xlsx");
            EnableViewState = false;
            Response.BinaryWrite(File.ReadAllBytes(excelFilename));
            File.Delete(excelFilename);
            Response.End();
        }
    }
}

using System;
using System.Data;
using ExportToExcel;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;
using System.Data.SqlClient;
using System.IO;
using Microsoft.ApplicationBlocks.Data;

namespace GRA.SRP.ControlRoom.Modules.Tenant
{
    public partial class TenantSummaryReport : BaseControlRoomPage
    {
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 8000;
            MasterPage.IsSecure = true; 
            
            if (!IsPostBack)
            {
                MasterPage.PageTitle = string.Format("{0}", "Tenant Summary Report");

                SetPageRibbon(StandardModuleRibbons.MasterTenantRibbon());

            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            rptr.DataSource = GetReportData();
            rptr.DataBind();
        }

        private DataSet GetReportData()
        {
            var arrParams = new SqlParameter[1];


            arrParams[0] = new SqlParameter("@IncSummary", chk.Checked );

            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "rpt_TenantReport", arrParams);

            return ds;
        } 

        protected void btnClear_Click(object sender, EventArgs e)
        {
            chk.Checked = false;
            rptr.DataSource = null;
            rptr.DataBind();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var ds = GetReportData();

            string excelFilename = Server.MapPath("~/TenantSummaryReport.xlsx");
            CreateExcelFile.CreateExcelDocument(ds, excelFilename);

            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("Content-Disposition", "attachment; filename=TenantSummaryReport.xlsx");
            EnableViewState = false;
            Response.BinaryWrite(File.ReadAllBytes(excelFilename));
            File.Delete(excelFilename);
            Response.End();
        }
    }
}

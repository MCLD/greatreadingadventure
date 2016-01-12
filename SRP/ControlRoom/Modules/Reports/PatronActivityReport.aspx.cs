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
    public partial class PatronActivityReport : BaseControlRoomPage
    {
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4200;
            MasterPage.IsSecure = true; 
            
            if (!IsPostBack)
            {
                MasterPage.PageTitle = string.Format("{0}", "Patron Activity Report");

                SetPageRibbon(StandardModuleRibbons.ReportsRibbon());

            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            rptr.DataSource = GetReportData();
            rptr.DataBind();
        }

        private DataSet GetReportData()
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
            arrParams[4] = new SqlParameter("@TenID", CRTenantID == null ? -1 : CRTenantID);

            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "rpt_PatronActivity", arrParams);

            return ds;
        } 

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ProgID.SelectedValue = BranchID.SelectedValue = "0";
            School.SelectedValue = LibSys.SelectedValue= string.Empty;
            rptr.DataSource = null;
            rptr.DataBind();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var ds = GetReportData();

            string excelFilename = Server.MapPath("~/PatronActivityReport.xlsx");
            CreateExcelFile.CreateExcelDocument(ds, excelFilename);

            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("Content-Disposition", "attachment; filename=PatronActivityReport.xlsx");
            EnableViewState = false;
            Response.BinaryWrite(File.ReadAllBytes(excelFilename));
            File.Delete(excelFilename);
            Response.End();
        }
    }
}

﻿using System;
using System.Data;
using ExportToExcel;
using SRPApp.Classes;
using STG.SRP.Core.Utilities;
using System.Data.SqlClient;
using System.IO;
using Microsoft.ApplicationBlocks.Data;

namespace STG.SRP.ControlRoom.Modules.Reports
{
    public partial class RegStats : BaseControlRoomPage
    {
        private static string conn = STG.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MasterPage.IsSecure = true;
                MasterPage.PageTitle = string.Format("{0}", "Registration Statistics Report");

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

            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "rpt_RegistrationStats", arrParams);

            return ds;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ProgID.SelectedValue = BranchID.SelectedValue = "0";
            School.SelectedValue = LibSys.SelectedValue = "";
            rptr.DataSource = null;
            rptr.DataBind();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var ds = GetReportData();

            string excelFilename = Server.MapPath("~/RegistrationStatsReportResults.xlsx");
            CreateExcelFile.CreateExcelDocument(ds, excelFilename);

            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("Content-Disposition", "attachment; filename=RegistrationStatsReportResults.xlsx");
            EnableViewState = false;
            Response.BinaryWrite(File.ReadAllBytes(excelFilename));
            File.Delete(excelFilename);
            Response.End();
        }
    }
}

using ExportToExcel;
using GRA.SRP.Core.Utilities;
using Microsoft.ApplicationBlocks.Data;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace GRA.SRP.ControlRoom.Modules.Reports
{
    public partial class EventCodesByBranch : BaseControlRoomPage
    {
        // reluctantly following the reporting pattern from the rest of the CR
        // TODO move all reporting database stuff to the DAL!
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        protected string HiddenColumnStyle { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.PageTitle = "Event Codes Report";
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
                if (EventRepeater.Items.Count == 0)
                {
                    ReportPanel.Visible = false;
                }
                else
                {
                    ReportPanel.Visible = true;
                }
            }
        }

        private DataSet PrepareReport(bool forExport = false)
        {
            var branches = new List<int>();
            int branchId = 0;
            int.TryParse(LibraryBranchList.SelectedValue, out branchId);

            int systemId = 0;
            int.TryParse(LibraryDistrictList.SelectedValue, out systemId);
            if (branchId == 0 && systemId == 0)
            {
                throw new Exception("Please choose a system or branch.");
            }
            PrintHeader.Text = string.Format("{0} events with secret codes for {1}, {2}",
                ShowAllDropdown.SelectedValue == "1" ? "All" : "Visible",
                LibraryDistrictList.SelectedItem.Text,
                LibraryBranchList.SelectedItem.Text);

            if (branchId == 0)
            {
                // report on system id
                // pull branches from system
                var cw = DAL.LibraryCrosswalk.GetFilteredBranchDDValues(systemId, string.Empty);
                if (cw != null && cw.Tables.Count > 0 && cw.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in cw.Tables[0].Rows)
                    {
                        branchId = row["CID"] as int? ?? 0;
                        if (branchId != 0)
                        {
                            branches.Add(branchId);
                        }
                    }
                }
                else
                {
                    throw new Exception("Could not find any branches in that system.");
                }
            }
            else
            {
                branches.Add(branchId);
            }

            var query = new StringBuilder("SELECT");
            if (ShowAllDropdown.SelectedValue == "1")
            {
                query.Append(" [HiddenFromPublic],");
            }
            else
            {
                if (!forExport)
                {
                    query.Append(" '' [HiddenFromPublic],");
                }
            }

            query.Append(" [EventTitle], [EventDate], [SecretCode] FROM [Event] WHERE [TenID] = @TenID AND [BranchID] IN (");
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@TenID", CRTenantID == null ? -1 : CRTenantID));

            int branchCount = 0;
            foreach (int selectedBranchId in branches)
            {
                if (branchCount > 0)
                {
                    query.Append(",");
                }
                query.Append("@BranchId");
                query.Append(branchCount);
                parameters.Add(new SqlParameter(
                    string.Format("@BranchId{0}", branchCount),
                    selectedBranchId
                    ));
                branchCount++;
            }

            query.Append(")");
            if (ShowAllDropdown.SelectedValue == "0")
            {
                query.Append(" AND (HiddenFromPublic IS NULL OR HiddenFromPublic != 1)");
            }
            query.Append(" ORDER BY [EventDate]");

            var eventData = SqlHelper.ExecuteDataset(conn,
                CommandType.Text,
                query.ToString(),
                parameters.ToArray());

            if (eventData != null && eventData.Tables.Count > 0 && eventData.Tables[0].Rows.Count > 0)
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
                if (ShowAllDropdown.SelectedValue == "0")
                {
                    HiddenColumnStyle = "display: none;";
                }
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
                var ds = PrepareReport(true);
                if (ds.Tables != null && ds.Tables.Count >= 1 && ds.Tables[0].Rows.Count != 0)
                {
                    ds.Tables[0].TableName = string.Format("{0}+Events+Codes",
                        ShowAllDropdown.SelectedValue == "1" ? "All" : "Visible");

                    CreateExcelFile.CreateExcelDocument(
                        ds,
                        string.Format("{0}-{1}-EventCodes.xlsx",
                            LibraryDistrictList.SelectedItem.Text.Replace(" ", string.Empty),
                            LibraryBranchList.SelectedItem.Text.Replace(" ", string.Empty)),
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

        protected void SelectedDistrict(object sender, EventArgs e)
        {
            LibraryBranchData.Select();
            LibraryBranchList.Items.Clear();
            LibraryBranchList.Items.Add(new ListItem(
                "All library branches",
                "0"));
            LibraryBranchList.DataBind();
            if (LibraryBranchList.Items.Count == 2)
            {
                LibraryBranchList.SelectedIndex = 1;
            }
        }
    }
}

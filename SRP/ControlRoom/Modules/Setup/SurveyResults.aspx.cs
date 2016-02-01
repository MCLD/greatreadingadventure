using System;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using ExportToExcel;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class SurveyResults : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

            }

            MasterPage.RequiredPermission = 5200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Test/Survey Results");
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            pnlResults.Visible = false;
            if (SID.SelectedValue == "0")
            {
                ddSource.Items.Clear();
                ddSource.Items.Add(new ListItem("[Select a Source for Additional Filtering]", "0"));
                MasterPage.PageError = "You need to select a Test/Survey.";
                return;
            }
            //if (ddSource.Items.Count <= 1)
            //{
            //    LoadSources();
            //}

            LoadData();
            pnlResults.Visible = true;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            SID.SelectedValue = "0";
            pnlResults.Visible = false;
            lblInfo.Text= string.Empty;
            ddSource.Items.Clear();
            ddSource.Items.Add(new ListItem("[Select a Source for Additional Filtering]", "0"));
        }

        public void LoadSources()
        {
            var currValue = ddSource.SelectedValue;
            ddSource.Items.Clear();
            ddSource.Items.Add(new ListItem("[Select a Source for Additional Filtering]", "0"));
            var ds = DAL.SurveyResults.GetSources(int.Parse(SID.SelectedValue));
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                ddSource.Items.Add(new ListItem(string.Format("{0} - {1}", r["Source"], r["SourceName"]), string.Format("{0}_|_{1}", r["Source"], r["SourceID"])));
            }

        }

        protected void LoadData()
        {
            var sid = int.Parse(SID.SelectedValue);
            string SourceType= string.Empty;
            int SourceID = 0;
            int SchoolID = 0;

            if (ddSource.SelectedValue == "0")
            {
                SourceType= string.Empty;
                SourceID = 0;
            }
            else
            {
                string[] stringSeparators = new string[] { "_|_" };
                var arr = ddSource.SelectedValue.Split(stringSeparators, StringSplitOptions.None);
                SourceType = arr[0];
                SourceID = int.Parse(arr[1]);
            }

            int.TryParse(ddSchool.SelectedValue, out SchoolID);

            var s = Survey.FetchObject(sid);
            var sr = DAL.SurveyResults.GetStats(sid, SourceType, SourceID, SchoolID);

            lblInfo.Text = string.Format("The test/survey has been taken {0} times by {1} different patrons.", Convert.ToInt32(sr.Tables[0].Rows[0][0]), Convert.ToInt32(sr.Tables[1].Rows[0][0]));

            rptr.DataSource = sr.Tables[2];
            rptr.DataBind();
        }



        protected void btnExport_Click(object sender, EventArgs e)
        {
            pnlResults.Visible = false;
            if (SID.SelectedValue == "0")
            {
                MasterPage.PageError = "You need to select a Test/Survey.";
                return;
            }
            LoadData();
            ExportData();
            pnlResults.Visible = true;
        }

        public void ExportData()
        {
            var sid = int.Parse(SID.SelectedValue);
            string SourceType= string.Empty;
            int SourceID = 0;
            int SchoolID = 0;

            if (ddSource.SelectedValue == "0")
            {
                SourceType= string.Empty;
                SourceID = 0;
            }
            else
            {
                string[] stringSeparators = new string[] { "_|_" };
                var arr = ddSource.SelectedValue.Split(stringSeparators, StringSplitOptions.None);
                SourceType = arr[0];
                SourceID = int.Parse(arr[1]);
            }
            int.TryParse(ddSchool.SelectedValue, out SchoolID);

            var ds = DAL.SurveyResults.GetExport(sid, SourceType, SourceID, SchoolID);

            string excelFilename = Server.MapPath("~/SurveyOrTestResultsExport.xlsx");
            CreateExcelFile.CreateExcelDocument(ds, excelFilename);

            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("Content-Disposition", "attachment; filename=SurveyOrTestResultsExport.xlsx");
            EnableViewState = false;
            Response.BinaryWrite(File.ReadAllBytes(excelFilename));
            File.Delete(excelFilename);
            Response.End();
        }

        protected void SID_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSources();
        }

    }

}
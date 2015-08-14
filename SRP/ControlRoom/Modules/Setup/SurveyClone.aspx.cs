using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class SurveyClone : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
                SID.Text = Session["SID"] == null ? "" : Session["SID"].ToString();
                if (SID.Text == "") Response.Redirect("SurveyList.aspx");
                var s = Survey.FetchObject(int.Parse(SID.Text));
                lblSurvey.Text = string.Format("{0} - {1}", s.Name, s.LongName);
                
            }
            MasterPage.RequiredPermission = 5200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Survey/Test Clone");
        }

        protected void btnCancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("SurveyList.aspx");
        }

        protected void btnClone_Click(object sender, ImageClickEventArgs e)
        {
            var so = Survey.FetchObject(int.Parse(SID.Text));
            var sn = Survey.FetchObject(int.Parse(SID.Text));
            sn.SID = 0;
            sn.Name = txtNewName.Text;
            sn.Status = 1;
            sn.Insert();

            var ds1 = SurveyQuestion.GetAll(so.SID);
            foreach (DataRow r1 in ds1.Tables[0].Rows)
            {
                var QID = Convert.ToInt32(r1["QID"]);

                var q = SurveyQuestion.FetchObject(QID);
                q.SID = sn.SID;
                q.QID = 0;
                q.Insert();

                var ds2 = SQChoices.GetAll(QID);
                foreach (DataRow r2 in ds2.Tables[0].Rows)
                {
                    var SQCID = Convert.ToInt32(r2["SQCID"]);
                    var c = SQChoices.FetchObject(SQCID);
                    c.SQCID = 0;
                    c.QID = q.QID;
                    c.Insert();
                }

                var ds3 = SQMatrixLines.GetAll(QID);
                foreach (DataRow r3 in ds3.Tables[0].Rows)
                {
                    var SQMLID = Convert.ToInt32(r3["SQMLID"]);
                    var l = SQMatrixLines.FetchObject(SQMLID);
                    l.SQMLID = 0;
                    l.QID = q.QID;
                    l.Insert();
                }
            }
            Session["SID"] = sn.SID;
            Response.Redirect("SurveyList.aspx");
            Response.Redirect("SurveyAddEdit.aspx");

        }
    }
}
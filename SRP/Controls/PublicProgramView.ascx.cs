using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.Classes
{
    public partial class PublicProgramView : System.Web.UI.UserControl
    {
        public Programs CurrentProgram { get; set; }

        private void LoadProgram()
        {
            this.CurrentProgram = Programs.FetchObject(Session["ProgramID"].ToString().SafeToInt());
            ViewState["Program"] = this.CurrentProgram;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProgram();

                DateTime startDate = this.CurrentProgram.StartDate < this.CurrentProgram.LoggingStart
                    ? this.CurrentProgram.StartDate
                    : this.CurrentProgram.LoggingStart;

                DateTime endDate = this.CurrentProgram.EndDate > this.CurrentProgram.LoggingEnd
                    ? this.CurrentProgram.EndDate
                    : this.CurrentProgram.LoggingEnd;

                if (DateTime.Now < startDate)
                {
                    RegisterLoginPanel.Visible = false;
                    NotYetPanel.Visible = true;
                    var notStarted = StringResources.getString("program-not-started");
                    NotStartedField.Text = notStarted.Contains("{0}")
                        ? string.Format(notStarted, startDate.ToLongDateString())
                        : notStarted;
                }
                else if (DateTime.Now > endDate)
                {
                    RegisterLoginPanel.Visible = false;
                    AlreadyOverPanel.Visible = true;
                    var alreadyOver = StringResources.getString("program-already-over");
                    ProgramAlreadyOver.Text = alreadyOver.Contains("{0}")
                        ? string.Format(alreadyOver, endDate.ToLongDateString())
                        : alreadyOver;
                }
            }
            else {
                if (ViewState["Program"] == null)
                {
                    LoadProgram();
                }
                else {
                    this.CurrentProgram = (Programs)ViewState["Program"];
                }
            }
        }
    }
}
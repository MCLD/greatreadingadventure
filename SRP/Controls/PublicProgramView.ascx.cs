using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.Classes {
    public partial class PublicProgramView : System.Web.UI.UserControl {
        public Programs CurrentProgram { get; set; }

        private void LoadProgram() {
            this.CurrentProgram = Programs.FetchObject(Session["ProgramID"].ToString().SafeToInt());
            ViewState["Program"] = this.CurrentProgram;
        }
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                LoadProgram();
            } else {
                if(ViewState["Program"] == null) {
                    LoadProgram();
                } else {
                    this.CurrentProgram = (Programs)ViewState["Program"];
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.Controls;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP {
    public partial class MyProgram : BaseSRPPage {
        protected void Page_Load(object sender, EventArgs e) {
            IsSecure = true;

            var patron = Session["Patron"] as Patron;
            if(patron == null) {
                Response.Redirect("~");
            }

            var pgm = DAL.Programs.FetchObject(patron.ProgID);
            if(pgm == null) {
                pgm = Programs.FetchObject(Programs.GetDefaultProgramForAgeAndGrade(patron.Age, patron.SchoolGrade.SafeToInt()));
                if(patron.ProgID != pgm.PID) {
                    patron.ProgID = pgm.PID;
                    patron.Update();
                }
            }

            if(pgm.IsOpen) {
                ProgramNotOpenText.Visible = false;
            } else {
                ProgramNotOpenText.Visible = true;
                ProgramNotOpenText.Text = Server.HtmlDecode(pgm.HTML6);
            }

            if(pgm != null) {
                if(!string.IsNullOrWhiteSpace(pgm.HTML2)) {
                    SponsorText.Text = Server.HtmlDecode(pgm.HTML2);
                    SponsorText.Visible = true;
                } else {
                    SponsorText.Visible = false;
                }

                if(!string.IsNullOrWhiteSpace(pgm.HTML5)) {
                    FooterText.Text = string.Format("<hr>{0}", Server.HtmlDecode(pgm.HTML5));
                    FooterText.Visible = true;
                } else {
                    FooterText.Visible = false;
                }
            }

            if(!IsPostBack)
                TranslateStrings(this);
        }
    }
}
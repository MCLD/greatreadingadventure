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

namespace GRA.SRP
{
    public partial class MyProgram : BaseSRPPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IsSecure = true;
            

            Control ctl = null;
            PlaceHolder plc = null;

            var pgm = DAL.Programs.FetchObject(int.Parse(Session["PatronProgramID"].ToString()));
            if (pgm == null)
            {
                var p = (Patron)Session["Patron"];
                pgm = Programs.FetchObject(Programs.GetDefaultProgramForAgeAndGrade(p.Age, p.SchoolGrade.SafeToInt()));
                if (p.ProgID != pgm.PID)
                {
                    p.ProgID = pgm.PID;
                    p.Update();
                }
                Session["PatronProgramID"] = Session["PatronProgramID"] = Session["CurrentProgramID"] = pgm.PID;

            }
            if (pgm.IsOpen)
            {
                ProgramNotOpenText.Visible = false;
            }
            else
            {
                ProgramNotOpenText.Visible = true;
                ProgramNotOpenText.Text = pgm.HTML6;
            }

            if(pgm != null) {
                if(!string.IsNullOrWhiteSpace(pgm.HTML2)) {
                    SponsorText.Text = pgm.HTML2;
                    SponsorText.Visible = true;
                } else {
                    SponsorText.Visible = false;
                }

                if(!string.IsNullOrWhiteSpace(pgm.HTML5)) {
                    FooterText.Text = string.Format("<hr>{0}", pgm.HTML5);
                    FooterText.Visible = true;
                } else {
                    FooterText.Visible = false;
                }
            }

            if (!IsPostBack) TranslateStrings(this);
        }
    }
}
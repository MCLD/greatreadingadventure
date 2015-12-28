
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP
{
    public partial class EnterFamMemberLog : BaseSRPPage
    {
       protected void Page_PreRender(object sender, EventArgs e) {
            var patron = Session["Patron"] as Patron;
            if(patron == null) {
                Response.Redirect("~");
            }

            var pgm = DAL.Programs.FetchObject(patron.ProgID);
            if(pgm == null) {
                pgm = Programs.FetchObject(
                    Programs.GetDefaultProgramForAgeAndGrade(patron.Age,
                                                             patron.SchoolGrade.SafeToInt()));
            }

            if(pgm == null || !pgm.IsOpen) {
                NotYet.Text = pgm.HTML6;
                NotYet.Visible = true;
                FamilyCodeControl.Visible = false;
            } else {
                NotYet.Visible = false;
                FamilyCodeControl.Visible = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            IsSecure = true;
            if(!IsPostBack) {
                TranslateStrings(this);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using STG.SRP.Controls;
using STG.SRP.DAL;
using STG.SRP.Utilities.CoreClasses;

namespace STG.SRP
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
                ctl = LoadControl("~/Controls/MyProgramLeftColumn.ascx");
                plc = LeftColumn;
                plc.Controls.Add(ctl);
                ((MyProgramLeftColumn) ctl).GamemapNavControl.Visible = (pgm.ProgramGameID != 0);

                ctl = LoadControl("~/Controls/MyProgramRightColumn.ascx");
                plc = RightColumn;
                plc.Controls.Add(ctl);

                if (pgm.ProgramGameID == 0)
                {
                    ctl = LoadControl("~/Controls/MyNoGameProgramCenterColumn.ascx");
                }
                else
                {
                    ctl = LoadControl("~/Controls/MyWithGameProgramCenterColumn.ascx");
                }
                plc = CenterColumn;
                plc.Controls.Add(ctl);
            }
            else
            {
                ctl = new Label();
                ((Label) ctl).Text = "<h2>"+pgm.Title+"</h2><hr>" + pgm.HTML6;
                plc = CenterColumn;
                plc.Controls.Add(ctl);
            }

            if (!IsPostBack) TranslateStrings(this);
        }
    }
}
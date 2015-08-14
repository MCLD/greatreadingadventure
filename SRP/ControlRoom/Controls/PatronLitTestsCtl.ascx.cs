using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Controls;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Controls
{
    public partial class PatronLitTestsCtl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                LoadData();

            }
        }

        public void LoadData()
        {
            var p = (Patron)Session["Curr_Patron"];
            var pg = Programs.FetchObject(p.ProgID);

            if (pg.PreTestID != 0)
            {
                if (p.Score1Date.ToWidgetDisplayDate() == "")
                {
                    lblT1S.Text = lblT1P.Text = lblT1R.Text = lblT1D.Text = "N/A";
                }
                else
                {
                    lblT1S.Text = string.Format("{0}", p.Score1);
                    lblT1P.Text = string.Format("{0} %", p.Score1Pct);
                    lblT1D.Text = string.Format("{0}", p.Score1Date.ToWidgetDisplayDate());

                    lblT1R.Text = Patron.GetTestRank(p.PID, 1);
                }
            }
            else
            {
                lblT1S.Text = lblT1P.Text = lblT1R.Text = lblT1D.Text = "N/A";
            }

            if (pg.PostTestID != 0)
            {
                if (p.Score2Date.ToWidgetDisplayDate() == "")
                {
                    lblT2S.Text = lblT2P.Text = lblT2R.Text = lblT2D.Text = "N/A";
                }
                else
                {
                    lblT2S.Text = string.Format("{0}", p.Score2);
                    lblT2P.Text = string.Format("{0} %", p.Score2Pct);
                    lblT2D.Text = string.Format("{0}", p.Score2Date.ToWidgetDisplayDate());

                    lblT2R.Text = Patron.GetTestRank(p.PID, 2);
                }
            }
            else
            {
                lblT2S.Text = lblT2P.Text = lblT2R.Text = lblT2D.Text = "N/A";
            }
        }
    }
}
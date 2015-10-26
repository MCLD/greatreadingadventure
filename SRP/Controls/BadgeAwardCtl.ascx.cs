using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Controls
{
    public partial class BadgeAwardCtl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request["b"]))
            {
                Response.Redirect("~/Dashboard.aspx");
            }

            if (Session["GoToUrl"] != null && Session["GoToUrl"].ToString() == "")
            {
                GoToUrl = Session["GoToUrl"].ToString();
                Session["GoToUrl"] = "";
            }

            Populate(Request["b"].ToString().Replace('|', ','));
        }


        void Populate(string IDs)
        {
            var ds = DAL.Badge.GetList(IDs);
            rptr.DataSource = ds;
            rptr.DataBind();

            Title.Text = "Congratulations, you have earned a badge!";
            var badgesEarned = ds.Tables[0].Rows.Count;
            if (badgesEarned != 1) Title.Text = string.Format("Congratulations, you have earned {0} badges!", badgesEarned.ToString());

        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            Response.Redirect(GoToUrl);
        }

        public string GoToUrl
        {
            get
            {
                if (ViewState["gotourl"] == null || ViewState["gotourl"].ToString().Length == 0)
                {
                    ViewState["gotourl"] = "~/Dashboard.aspx";
                }
                return ViewState["gotourl"].ToString();
            }
            set { ViewState["gotourl"] = value; }
        }
    }
}
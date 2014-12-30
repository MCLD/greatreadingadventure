using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using STG.SRP.DAL;

namespace STG.SRP.Controls
{
    public partial class FamilyList : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // now validate user can change password for SA Sub Account

                var patron = (Patron)Session["Patron"];
                if (!patron.IsMasterAccount)
                {
                    // kick them out
                    Response.Redirect("~/Logout.aspx");
                }

                // populate screen
                var ds = Patron.GetSubAccountList(patron.PID);
                rptr.DataSource = ds;
                rptr.DataBind();
                ((BaseSRPPage)Page).TranslateStrings(rptr);

            }
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Session["SA"] = "0";

            if (e.CommandName.ToLower() == "childadd")
            {
                Response.Redirect("~/AddChildAccount.aspx");
            }

            if (e.CommandName == "pwd")
            {
                Session["SA"] = e.CommandArgument.ToString();
                Response.Redirect("~/ChangeFamMemberPwd.aspx");

            }
            if (e.CommandName == "act")
            {
                Session["SA"] = e.CommandArgument.ToString();
                Response.Redirect("~/ChangeFamMemberAct.aspx");

            }
            if (e.CommandName == "log")
            {
                Session["SA"] = e.CommandArgument.ToString();
                Response.Redirect("~/EnterFamMemberLog.aspx");

            }
        }

        protected void btn_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AddChildAccount.aspx");
        }
    }
}
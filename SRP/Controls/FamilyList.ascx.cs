using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;
using GRA.Tools;

namespace GRA.SRP.Controls
{
    public partial class FamilyList : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                var patron = (Patron)Session["Patron"];
                if (Session[SessionKey.IsMasterAccount] == null || !(bool)Session[SessionKey.IsMasterAccount])
                {
                    Response.Redirect("~/Logout.aspx");
                }

                // populate screen
                var ds = Patron.GetSubAccountList((int)Session["MasterAcctPID"]);
                rptr.DataSource = ds;
                rptr.DataBind();

                var parent = Patron.FetchObject((int)Session["MasterAcctPID"]);
                ddAccounts.Items.Add(new ListItem(string.Format("{0} {1} ({2})", parent.FirstName, parent.LastName, parent.Username),parent.PID.ToString()));

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var value = Convert.ToString(row["PID"]);
                    var text = string.Format("{0} {1} ({2})", Convert.ToString(row["FirstName"]), Convert.ToString(row["LastName"]), Convert.ToString(row["Username"]));

                    ddAccounts.Items.Add(new ListItem(text, value));
                }


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

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            pnlList.Visible = false;
            pnlChange.Visible = true;
        }

        protected void btncancel_Click(object sender, EventArgs e)
        {
            pnlList.Visible = true;
            pnlChange.Visible = false;
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            var newPID = int.Parse(ddAccounts.SelectedValue);

            if ((int)Session["MasterAcctPID"] != newPID && !Patron.CanManageSubAccount((int)Session["MasterAcctPID"], newPID))
            {
                // kick them out
                Response.Redirect("~/Logout.aspx");
            }

            var bp = Patron.FetchObject(newPID);
            Session["Patron"] = bp;
            Session["ProgramID"] = bp.ProgID;
            Session["PatronProgramID"] = bp.ProgID;
            Session["CurrentProgramID"] = bp.ProgID;
            Session["TenantID"] = bp.TenID;

            Response.Redirect("~/Dashboard.aspx");
        }
    }
}
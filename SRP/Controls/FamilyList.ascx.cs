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

namespace GRA.SRP.Controls {
    public partial class FamilyList : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {

                var patron = (Patron)Session["Patron"];
                if(Session[SessionKey.IsMasterAccount] as bool? != true) {
                    Response.Redirect("~/Dashboard.aspx");
                }

                // populate screen
                var ds = Patron.GetSubAccountList((int)Session["MasterAcctPID"]);
                rptr.DataSource = ds;
                rptr.DataBind();

                ((BaseSRPPage)Page).TranslateStrings(rptr);
            }
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e) {
            Session["SA"] = "0";
            if(e.CommandName == "pwd") {
                Session["SA"] = e.CommandArgument.ToString();
                Response.Redirect("~/Account/ChangeFamMemberPwd.aspx");

            }
            if(e.CommandName == "act") {
                Session["SA"] = e.CommandArgument.ToString();
                Response.Redirect("~/Account/ChangeFamMemberAct.aspx");

            }
            if(e.CommandName == "log") {
                Session["SA"] = e.CommandArgument.ToString();
                Response.Redirect("~/Account/EnterFamMemberLog.aspx");

            }
            if(e.CommandName == "login") {
                var newPID = int.Parse(e.CommandArgument.ToString());

                if((int)Session["MasterAcctPID"] != newPID
                   && !Patron.CanManageSubAccount((int)Session["MasterAcctPID"], newPID)) {
                    // kick them out
                    Response.Redirect("~/Dashboard.aspx");
                }

                var newPatron = Patron.FetchObject(newPID);
                new PatronSession(Session).Establish(newPatron);
                //Session["Patron"] = bp;
                //Session["ProgramID"] = bp.ProgID;
                //Session["PatronProgramID"] = bp.ProgID;
                //Session["CurrentProgramID"] = bp.ProgID;
                //Session["TenantID"] = bp.TenID;

                Response.Redirect("~/Dashboard.aspx");
            }
        }
    }
}
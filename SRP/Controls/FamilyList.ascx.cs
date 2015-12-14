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
using System.IO;

namespace GRA.SRP.Controls {
    public partial class FamilyList : System.Web.UI.UserControl {
        private const string NoAvatarPathSm = "~/images/Avatars/no_avatar_sm.png";

        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {

                var patron = (Patron)Session["Patron"];
                if(Session[SessionKey.IsMasterAccount] as bool? != true) {
                    Response.Redirect("~");
                }

                // populate screen
                var ds = Patron.GetSubAccountList((int)Session["MasterAcctPID"]);
                rptr.DataSource = ds;
                rptr.DataBind();

                ((BaseSRPPage)Page).TranslateStrings(rptr);
            }
        }

        protected void rptr_ItemDataBound(object source, RepeaterItemEventArgs e) {
            if(e.Item.ItemType == ListItemType.Item
               || e.Item.ItemType == ListItemType.AlternatingItem) {
                var patronRecord = (DataRowView)e.Item.DataItem;
                var avatarId = patronRecord["AvatarID"].ToString();

                string potentialAvatarPath = null;
                string avatarPathSm = NoAvatarPathSm;
                string avatarPathMd = null;

                potentialAvatarPath = string.Format("~/images/Avatars/sm_{0}.png",
                                                    avatarId);
                if(File.Exists(Server.MapPath(potentialAvatarPath))) {
                    avatarPathSm = potentialAvatarPath;
                }

                potentialAvatarPath = string.Format("~/images/Avatars/md_{0}.png",
                                                    avatarId);
                if(File.Exists(Server.MapPath(potentialAvatarPath))) {
                    avatarPathMd = potentialAvatarPath;
                }

                var image = (Image)e.Item.FindControl("avatarImage");
                image.ImageUrl = avatarPathSm;
                if(!string.IsNullOrEmpty(avatarPathMd)) {
                    string srcSet = string.Format("{0} 1x, {1} 2x",
                                                  VirtualPathUtility.ToAbsolute(avatarPathSm),
                                                  VirtualPathUtility.ToAbsolute(avatarPathMd));
                    image.Attributes.Add("srcset", srcSet);
                }
            }
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e) {
            Session["SA"] = "0";
            if(e.CommandName == "pwd") {
                Session["SA"] = e.CommandArgument.ToString();
                Response.Redirect("~/Account/ChangeFamMemberPwd.aspx");

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
                    Response.Redirect("~");
                }

                var newPatron = Patron.FetchObject(newPID);
                new SessionTools(Session).EstablishPatron(newPatron);
                //Session["Patron"] = bp;
                //Session["ProgramID"] = bp.ProgID;
                //Session["TenantID"] = bp.TenID;

                Response.Redirect("~");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;

namespace GRA.SRP.Controls {
    public partial class LeaderBoardControl : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                LoadData();
            }

        }

        protected string AvatarImage(string avatarId) {
            string avatarPath = "~/Images/Avatars/no_avatar_sm.png";
            string potentialAvatarPath = string.Format("~/Images/Badges/{0}_sm.png",
                                                      avatarId);
            if(System.IO.File.Exists(Server.MapPath(potentialAvatarPath))) {
                avatarPath = potentialAvatarPath;
            }
            return avatarPath;
        }

        public void LoadData() {
            var ds = DAL.Programs.GetLeaderboard(((Patron)Session["Patron"]).ProgID);
            rptr.DataSource = ds;
            rptr.DataBind();
        }

        protected void rptr_ItemDataBound(object sender, RepeaterItemEventArgs e) {
            if(e.Item.ItemType == ListItemType.Item
               || e.Item.ItemType == ListItemType.AlternatingItem) {
                var imageControl = (Image)e.Item.FindControl("SmallAvatar");
                string potentialAvatarPath = imageControl.ImageUrl;
                if(!System.IO.File.Exists(Server.MapPath(potentialAvatarPath))) {
                    imageControl.ImageUrl = "~/Images/Avatars/no_avatar_sm.png";
                }
            }
        }
    }
}
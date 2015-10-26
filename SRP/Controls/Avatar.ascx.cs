using GRA.SRP.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Controls {
    public partial class Avatar : System.Web.UI.UserControl {
        private const string NoAvatarPath = "~/images/Avatars/no_avatar.png";
        protected void Page_Load(object sender, EventArgs e) {
            var patron = (Patron)(Session["Patron"]);
            string avatarPath = NoAvatarPath;
            if(patron != null) {
                string potentialAvatarPath = string.Format("~/images/Avatars/{0}.png",
                                                           patron.AvatarID);
                if(File.Exists(Server.MapPath(potentialAvatarPath))) {
                    avatarPath = potentialAvatarPath;
                }
            }

            imgAvatar.ImageUrl = avatarPath;
        }
    }
}
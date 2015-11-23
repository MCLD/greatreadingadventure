using GRA.SRP.DAL;
using GRA.Tools;
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
        private const string NoAvatarPathSm = "~/images/Avatars/no_avatar_sm.png";

        protected void Page_Load(object sender, EventArgs e) {
            var patron = (Patron)(Session["Patron"]);
            string avatarPath = NoAvatarPath;
            string avatarPathSm = NoAvatarPathSm;
            if(patron != null) {
                string potentialAvatarPath = string.Format("~/images/Avatars/{0}.png",
                                                           patron.AvatarID);
                if(File.Exists(Server.MapPath(potentialAvatarPath))) {
                    avatarPath = potentialAvatarPath;
                }

                potentialAvatarPath = string.Format("~/images/Avatars/sm_{0}.png",
                                                    patron.AvatarID);
                if(File.Exists(Server.MapPath(potentialAvatarPath))) {
                    avatarPathSm = potentialAvatarPath;
                }
            }

            imgAvatar.ImageUrl = avatarPath;
            imgAvatarSm.ImageUrl = avatarPathSm;
            patronName.Text = DisplayHelper.FormatFirstName(patron.FirstName, patron.Username);
        }
    }
}
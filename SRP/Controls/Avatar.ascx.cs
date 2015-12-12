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
            string avatarPathMd = null;
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

                potentialAvatarPath = string.Format("~/images/Avatars/md_{0}.png",
                                                    patron.AvatarID);
                if(File.Exists(Server.MapPath(potentialAvatarPath))) {
                    avatarPathMd = potentialAvatarPath;
                }
            }

            imgAvatar.ImageUrl = avatarPath;
            imgAvatarSm.ImageUrl = avatarPathSm;
            if(!string.IsNullOrEmpty(avatarPathMd)) {
                string srcSet = string.Format("{0} 1x, {1} 2x",
                                              VirtualPathUtility.ToAbsolute(avatarPathSm),
                                              VirtualPathUtility.ToAbsolute(avatarPathMd));
                imgAvatarSm.Attributes.Add("srcset", srcSet);
            }
            patronName.Text = DisplayHelper.FormatFirstName(patron.FirstName, patron.Username);
        }
    }
}
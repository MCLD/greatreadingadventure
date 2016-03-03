using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using SRPApp.Classes;
using GRA.Tools;

namespace GRA.SRP.Controls {
    public partial class MyAvatarControl : System.Web.UI.UserControl {
        protected System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();

        public int layerCount = 1;
        public Dictionary<string, List<string>> components = new Dictionary<string, List<string>>();

        protected void Page_Load(object sender, EventArgs e) {


            var head = new List<string> {"/images/Avatars/1.png", "/images/Avatars/2.png", "/images/Avatars/3.png", "/images/Avatars/4.png" };


            components.Add("head", head);


            if(!IsPostBack) {

            }
        }

    }
}
using System;
using SRPApp.Classes;

namespace GRA.SRP {
    public partial class MyBadges : BaseSRPPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IsSecure = true;
            if (!IsPostBack) TranslateStrings(this);

        }
    }
}
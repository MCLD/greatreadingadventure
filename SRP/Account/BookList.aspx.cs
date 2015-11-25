using SRPApp.Classes;
using System;

namespace GRA.SRP {
    public partial class ViewBookList : BaseSRPPage {
        protected void Page_Load(object sender, EventArgs e) {
            IsSecure = true;
            if(!IsPostBack) {
                TranslateStrings(this);
            }
        }
    }
}
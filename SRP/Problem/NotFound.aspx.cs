using GRA.SRP.DAL;
using SRPApp.Classes;
using System;

namespace GRA.SRP {
    public partial class NotFound : BaseSRPPage {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                TranslateStrings(this);

                try {
                    string contactEmail = SRPSettings.GetSettingValue("ContactEmail");
                    if(!string.IsNullOrEmpty(contactEmail)) {
                        AlternateContact.Text = string.Format("If you continue to have issues, you can send an email to <a href=\"mailto:{0}\">{0}</a>.",
                                                              contactEmail);
                    }
                } catch(Exception ex) {
                    try {
                        this.Log().Error("An error occurred showing the not found page: {0}",
                                         ex.Message);
                    } catch(Exception) {

                    }
                }
            }
        }
    }
}
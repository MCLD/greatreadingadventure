﻿using GRA.SRP.DAL;

namespace GRA.SRP.Controls {
    public partial class Welcome : System.Web.UI.UserControl {
        public string WelcomeText {
            get {
                var patron = Session["Patron"] as Patron;
                if(patron == null) {
                    return "Welcome!";
                } else {
                    return string.Format("Welcome, {0}!", patron.FirstName);
                }
            }
        }
    }
}
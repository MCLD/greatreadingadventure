using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using STG.SRP.Controls;
using STG.SRP.DAL;

namespace STG.SRP
{
    public partial class SRPMaster : BaseSRPMaster
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            base.PageLoad(sender, e);

            //var systemName = SRPSettings.GetSettingValue("SystemName");
            //PageTitle = (string.IsNullOrEmpty(systemName) ? "Summer Reading Program" : systemName);

            Control ctl = LoadControl("~/Controls/ProgramCSS.ascx");
            var plc = FindControl("ProgramCSS");
            plc.Controls.Add(ctl);

            var thisPage = (BaseSRPPage) Page;
            if (thisPage.IsSecure && !thisPage.IsLoggedIn) Response.Redirect("~/Logout.aspx");


            if (!IsPostBack)
            {
                lnkRegister.Visible = true;
                lnkLogin.Visible = true;
                lnkLogout.Visible = false;
                n.Visible = b.Visible = v.Visible = o.Visible = a.Visible = p.Visible = f.Visible = false;
                home1.Visible = true;
                home2.Visible = false;

                if (thisPage.IsLoggedIn)
                {
                    lnkRegister.Visible = false;
                    lnkLogin.Visible = false;
                    lnkLogout.Visible = true;

                    home2.Visible = n.Visible = b.Visible = v.Visible = o.Visible = r.Visible = a.Visible = p.Visible = true;
                    home1.Visible = false;
                    //f.Visible = ((Patron) Session["Patron"]).IsMasterAccount;
                    if (Session["IsMasterAcct"] != null)
                        f.Visible = (bool)Session["IsMasterAcct"];


                    if (!(Page is AddlSurvey || Page is Register || Page is Login || Page is Logout || Page is Recover))
                    {
                        if (Session["PreTestMandatory"] != null && (bool)Session["PreTestMandatory"])
                        {
                            TestingBL.PatronNeedsPreTest();
                        }
                    }
                }

            }

        }
    }
}


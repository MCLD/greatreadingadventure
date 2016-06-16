using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using SRPApp.Classes;
using SRP_DAL;
using GRA.Tools;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.ControlRoom
{
    public partial class _default : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var crLoginHtml = DAL.SRPSettings.GetSettingValue("CRLoginHtml");
                if (!string.IsNullOrEmpty(crLoginHtml))
                {
                    CRLoginHtml.Text = crLoginHtml;
                    CRLoginHtml.Visible = true;
                }
                else
                {
                    CRLoginHtml.Visible = false;
                }

                var defaultRibbon = StandardModuleRibbons.DefaultRibbon();
                if (defaultRibbon.Count > 0)
                {
                    SetPageRibbon(defaultRibbon);
                }
            }
            if (CRTenantID != null)
            {
                var result = new TenantStatus((int)CRTenantID).CurrentStatus();
                RegisteredPatrons.Text = result.RegisteredPatrons.ToString("N0");
                PointsEarned.Text = result.PointsEarned.ToString("N0");
                PointsEarnedReading.Text = result.PointsEarnedReading.ToString("N0");
                ChallengesCompleted.Text = result.ChallengesCompleted.ToString("N0");
                AdventuresCompleted.Text = result.AdventuresCompleted.ToString("N0");
                BadgesAwarded.Text = result.BadgesAwarded.ToString("N0");
                SecretCodesRedeemed.Text = result.SecretCodesRedeemed.ToString("N0");
                ProgramCodesRedeemed.Text = result.RedeemedProgramCodes.ToString("N0");

                if (DAL.ProgramCodes.GetCountByTenantId((int)CRTenantID) > 0)
                {
                    ProgramCodesDiv.Visible = true;
                    var redeemed = result.SecretCodesRedeemed.ToString();
                    string codeName = StringResources.getString("myaccount-program-reward-code");
                    if (!codeName.EndsWith("s"))
                    {
                        codeName += "s";
                    }
                    ProgramRewardCodeLabel.Text = codeName;
                }
                else
                {
                    ProgramCodesDiv.Visible = false;
                }
                MasterPage.PageTitle = "At-a-glance Program Status - Items Earned by Patrons";

                var tenant = GRA.SRP.Core.Utilities.Tenant.FetchObject((int)CRTenantID);
                OrganizationName.Text = tenant.LandingName;

                var bannerPath = new Logic.Banner().GetBannerPath(
                    DAL.Programs.GetDefaultProgramID().ToString(),
                    Server);

                ProgramImage.ImageUrl = bannerPath.Item1;
                ProgramImage.CssClass = new WebTools().CssEnsureClass("img-rounded",
                    ProgramImage.CssClass);

                StatusPanel.Visible = true;
            }
            else
            {
                MasterPage.PageTitle = "Control Room";
                StatusPanel.Visible = false;
            }
        }
    }
}
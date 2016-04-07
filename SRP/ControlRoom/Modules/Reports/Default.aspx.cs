using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;
using SRP_DAL;
using GRA.Tools;

namespace GRA.SRP.ControlRoom.Modules.Reports
{
    public partial class Default : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4200;
            MasterPage.IsSecure = true;

            if (!IsPostBack)
            {
                MasterPage.PageTitle = string.Format("{0}", "At-a-glance Report");
                SetPageRibbon(StandardModuleRibbons.ReportsRibbon());
                var tenant = GRA.SRP.Core.Utilities.Tenant.FetchObject((int)CRTenantID);
                OrganizationName.Text = tenant.LandingName;
            }

            UpdateReport();
        }

        protected void UpdateReport()
        {
            var ts = new TenantStatus((int)CRTenantID);
            if (ProgramList.SelectedValue != "0")
            {
                ts.ProgramId = int.Parse(ProgramList.SelectedValue);
            }
            if (LibraryBranchList.SelectedValue != "0")
            {
                ts.BranchId = int.Parse(LibraryBranchList.SelectedValue);
            }
            else
            {
                if (LibraryDistrictList.SelectedValue != "0")
                {
                    ts.DistrictId = int.Parse(LibraryDistrictList.SelectedValue);
                }
            }
            var result = ts.CurrentStatus();
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
                ProgramRewardCodeLabel.Text = StringResources.getString("myaccount-program-reward-code");
            }
            else
            {
                ProgramCodesDiv.Visible = false;
            }

            ProgramName.Text = ProgramList.SelectedItem.Text;
            DistrictName.Text = LibraryDistrictList.SelectedItem.Text;
            BranchName.Text = LibraryBranchList.SelectedItem.Text;

            var defaultImage = true;
            if (ProgramList.SelectedValue != "0")
            {
                var program = DAL.Programs.FetchObject(int.Parse(ProgramList.SelectedValue));
                if (!string.IsNullOrWhiteSpace(program.BannerImage))
                {
                    defaultImage = false;
                    ProgramImage.ImageUrl = program.BannerImage;
                    ProgramImage.CssClass = new WebTools().CssRemoveClass("img-rounded", ProgramImage.CssClass);
                }
            }
            if (defaultImage)
            {
                var program = DAL.Programs.FetchObject(DAL.Programs.GetDefaultProgramID());
                if (!string.IsNullOrWhiteSpace(program.BannerImage))
                {
                    ProgramImage.ImageUrl = program.BannerImage;
                    ProgramImage.CssClass = new WebTools().CssRemoveClass("img-rounded", ProgramImage.CssClass);
                }
                else
                {
                    ProgramImage.ImageUrl = "~/images/meadow.jpg";
                    ProgramImage.CssClass = new WebTools().CssEnsureClass("img-rounded", ProgramImage.CssClass);
                }
            }
        }

        protected void DropDownDataBound(object sender, EventArgs e)
        {
            var ddl = sender as DropDownList;
            if (ddl != null && ddl.Items.Count == 1)
            {
                ddl.Visible = false;
            }
        }

        protected void SelectedDistrict(object sender, EventArgs e)
        {
            LibraryBranchData.Select();
            LibraryBranchList.Items.Clear();
            LibraryBranchList.Items.Add(new ListItem(
                "All library branches",
                "0"));
            LibraryBranchList.DataBind();
            if (LibraryBranchList.Items.Count == 2)
            {
                LibraryBranchList.SelectedIndex = 1;
            }
            BranchName.Text = LibraryBranchList.SelectedItem.Text;
            UpdateReport();
        }
    }
}

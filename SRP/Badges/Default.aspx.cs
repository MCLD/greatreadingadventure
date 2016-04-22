using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;
using GRA.Tools;

namespace GRA.SRP
{
    public partial class BadgeGallery : BaseSRPPage
    {
        public string NoneAvailableText { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request["PID"]))
            {
                Session["ProgramID"] = Request["PID"].ToString();
            }
            if (!IsPostBack)
            {
                if (Session["ProgramID"] == null)
                {
                    try
                    {
                        int PID = Programs.GetDefaultProgramID();
                        Session["ProgramID"] = PID.ToString();
                    }
                    catch
                    {
                        Response.Redirect("~/Default.aspx");
                    }
                }
            }
            TranslateStrings(this);
            this.NoneAvailableText = GetResourceString("badges-none-available");
            myBadgesButton.Visible = this.IsLoggedIn;
            this.MetaDescription = string.Format("See the available badges and how to earn them! - {0}",
                                                 GetResourceString("system-name"));
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            odsBadges.DataBind();
            rptr.DataBind();
            var wt = new WebTools();
            if (CategoryId.SelectedIndex > 0
                || AgeGroupId.SelectedIndex > 0
                || BranchId.SelectedIndex > 0
                || LocationID.SelectedIndex > 0)
            {
                CategoryId.CssClass = wt.CssEnsureClass(CategoryId.CssClass, "gra-search-active");
                AgeGroupId.CssClass = wt.CssEnsureClass(AgeGroupId.CssClass, "gra-search-active");
                BranchId.CssClass = wt.CssEnsureClass(BranchId.CssClass, "gra-search-active");
                LocationID.CssClass = wt.CssEnsureClass(LocationID.CssClass, "gra-search-active");
            }
            else
            {
                CategoryId.CssClass = wt.CssRemoveClass(CategoryId.CssClass, "gra-search-active");
                AgeGroupId.CssClass = wt.CssRemoveClass(AgeGroupId.CssClass, "gra-search-active");
                BranchId.CssClass = wt.CssRemoveClass(BranchId.CssClass, "gra-search-active");
                LocationID.CssClass = wt.CssRemoveClass(LocationID.CssClass, "gra-search-active");

            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            CategoryId.ClearSelection();
            AgeGroupId.ClearSelection();
            BranchId.ClearSelection();
            LocationID.ClearSelection();
            odsBadges.DataBind();
            rptr.DataBind();
            var wt = new WebTools();
            CategoryId.CssClass = wt.CssRemoveClass(CategoryId.CssClass, "gra-search-active");
            AgeGroupId.CssClass = wt.CssRemoveClass(AgeGroupId.CssClass, "gra-search-active");
            BranchId.CssClass = wt.CssRemoveClass(BranchId.CssClass, "gra-search-active");
            LocationID.CssClass = wt.CssRemoveClass(LocationID.CssClass, "gra-search-active");
        }

        protected void dd_DataBound(object sender, EventArgs e)
        {
            var dd = sender as DropDownList;
            if (dd.Items.Count < 2)
                dd.Visible = false;
        }
    }
}
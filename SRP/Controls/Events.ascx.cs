using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using System.Text;
using Newtonsoft.Json.Linq;
using GRA.Tools;

namespace GRA.SRP.Controls
{
    public partial class Events : System.Web.UI.UserControl
    {
        public string FirstAvailableDate { get; set; }
        public string LastAvailableDate { get; set; }
        public string NoneAvailableText { get; set; }
        public bool Filtered { get; set; }
        public string SelectLibrary { get; set; }
        public string SelectSystem { get; set; }
        public bool InitialFilter { get; set; }

        protected string DisplayEventDateTime(DateTime? eventDate)
        {
            if (eventDate != null)
            {
                return Event.DisplayEventDateTime(new Event
                {
                    EventDate = (DateTime)eventDate
                });
            }
            else
            {
                return string.Empty;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitialFilter = false;
                if (Request.QueryString.Count > 0)
                {
                    InitialFilter = PrepareFilter();
                    Page.PreRenderComplete += (s, args) =>
                    {
                        EnactFilter();
                    };
                }

                if (!InitialFilter)
                {
                    DoFilter();
                }
            }

            int programId;
            var sessionProgramId = Session["ProgramID"];
            if (sessionProgramId == null
               || !int.TryParse(sessionProgramId.ToString(), out programId))
            {
                programId = Programs.GetDefaultProgramID();

            }
            var program = Programs.FetchObject(programId);

            this.FirstAvailableDate = program.StartDate.ToShortDateString();
            this.LastAvailableDate = program.EndDate.ToShortDateString();

            var basePage = (BaseSRPPage)Page;
            this.NoneAvailableText = basePage.GetResourceString("events-none-available");
        }

        protected void EnactFilter()
        {
            if (!IsPostBack && InitialFilter)
            {
                if (!string.IsNullOrWhiteSpace(SelectLibrary))
                {
                    var branchItem = BranchId.Items.FindByValue(SelectLibrary);
                    if (branchItem != null)
                    {
                        BranchId.SelectedValue = SelectLibrary;
                    }
                }
                if (!string.IsNullOrWhiteSpace(SelectSystem))
                {
                    var systemItem = SystemId.Items.FindByValue(SelectSystem);
                    if (systemItem != null)
                    {
                        SystemId.SelectedValue = SelectSystem;
                    }
                }
                DoFilter();
            }
        }

        protected bool PrepareFilter()
        {
            bool filter = false;
            var qs = new Logic.QueryString();
            var querySystem = Request.QueryString["System"];
            if (!string.IsNullOrWhiteSpace(querySystem))
            {
                SelectSystem = qs.GetSystemIdString(Server.UrlDecode(querySystem));
                if(!string.IsNullOrEmpty(SelectSystem))
                {
                    filter = true;
                }
            }

            if (string.IsNullOrWhiteSpace(SelectSystem))
            {
                var queryBranch = Request.QueryString["Branch"];
                if (!string.IsNullOrWhiteSpace(queryBranch))
                {
                    var sysBranchId = qs.GetSystemBranchIdStrings(queryBranch);
                    if (sysBranchId != null)
                    {
                        SelectSystem = sysBranchId.Item1;
                        SelectLibrary = sysBranchId.Item2;
                        if(!string.IsNullOrEmpty(SelectSystem)
                           || !string.IsNullOrEmpty(SelectLibrary))
                        {
                            filter = true;
                        }
                    }
                }
            }

            var querySearch = Request.QueryString["Search"];
            if (!string.IsNullOrWhiteSpace(querySearch))
            {
                filter = true;
                if (querySearch.Length > 255)
                {
                    SearchText.Text = querySearch.Substring(0, 255);
                }
                else
                {
                    SearchText.Text = querySearch;
                }
            }
            return filter;
        }

        protected void DoFilter()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(StartDate.Text))
            {
                sb.Append("Start Date: ");
                sb.Append("<strong>");
                sb.Append(StartDate.Text);
                sb.Append("</strong>");
            }
            if (!string.IsNullOrEmpty(EndDate.Text))
            {
                if (sb.Length > 0)
                {
                    sb.Append(" / ");
                }
                sb.Append("End date: ");
                sb.Append("<strong>");
                sb.Append(EndDate.Text);
                sb.Append("</strong>");
            }
            if (SystemId.SelectedIndex > 0)
            {
                if (sb.Length > 0)
                {
                    sb.Append(" / ");
                }
                sb.Append("System: ");
                sb.Append("<strong>");
                sb.Append(SystemId.SelectedItem.Text);
                sb.Append("</strong>");
            }
            if (BranchId.SelectedIndex > 0)
            {
                if (sb.Length > 0)
                {
                    sb.Append(" / ");
                }
                sb.Append("Branch/library: ");
                sb.Append("<strong>");
                sb.Append(BranchId.SelectedItem.Text);
                sb.Append("</strong>");
            }
            if (SearchText.Text.Length > 0)
            {
                if (sb.Length > 0)
                {
                    sb.Append(" / ");
                }
                sb.Append("Search text: ");
                sb.Append("<strong>");
                sb.Append(SearchText.Text);
                sb.Append("</strong>");
            }

            WhatsShowing.Text = WhatsShowingPrint.Text = sb.ToString();
            WhatsShowingPanel.Visible = Filtered = !string.IsNullOrEmpty(WhatsShowing.Text);

            rptr.DataSource = Event.GetUpcomingDisplay(
                StartDate.Text,
                EndDate.Text,
                int.Parse(SystemId.SelectedValue),
                int.Parse(BranchId.SelectedValue),
                SearchText.Text
                );
            rptr.DataBind();

            var wt = new WebTools();
            if (Filtered)
            {
                StartDate.CssClass = wt.CssEnsureClass(StartDate.CssClass, "gra-search-active");
                EndDate.CssClass = wt.CssEnsureClass(EndDate.CssClass, "gra-search-active");
                BranchId.CssClass = wt.CssEnsureClass(BranchId.CssClass, "gra-search-active");
                SystemId.CssClass = wt.CssEnsureClass(SystemId.CssClass, "gra-search-active");
                SearchText.CssClass = wt.CssEnsureClass(SearchText.CssClass, "gra-search-active");
            }
            else
            {
                StartDate.CssClass = wt.CssRemoveClass(StartDate.CssClass, "gra-search-active");
                EndDate.CssClass = wt.CssRemoveClass(EndDate.CssClass, "gra-search-active");
                BranchId.CssClass = wt.CssRemoveClass(BranchId.CssClass, "gra-search-active");
                SystemId.CssClass = wt.CssRemoveClass(SystemId.CssClass, "gra-search-active");
                SearchText.CssClass = wt.CssRemoveClass(SearchText.CssClass, "gra-search-active");
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            DoFilter();
        }

        protected void ClearSelections()
        {
            StartDate.Text
                = EndDate.Text
                = SearchText.Text
                = WhatsShowing.Text
                = WhatsShowingPrint.Text
                = string.Empty;
            WhatsShowingPanel.Visible = Filtered = false;
            SystemId.SelectedValue = "0";
            UpdateBranchList();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearSelections();
            DoFilter();
        }

        protected void btnList_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Events/");
        }

        protected void rptr_ItemDataBound(object source, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item
                || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var eventRow = e.Item.DataItem as System.Data.DataRowView;
                var branchName = eventRow["Branch"].ToString();
                var branchAddress = eventRow["BranchAddress"];
                var branchTelephone = eventRow["BranchTelephone"];
                var branchLink = eventRow["BranchLink"];
                var label = e.Item.FindControl("BranchName") as Label;

                bool haveLink = branchLink != null
                    && !string.IsNullOrWhiteSpace(branchLink.ToString());
                bool haveAddress = branchAddress != null
                    && !string.IsNullOrWhiteSpace(branchAddress.ToString());

                DateTime eventDate = DateTime.MinValue;
                if (eventRow["EventDate"] != null)
                {
                    eventDate = eventRow["EventDate"] as DateTime? ?? DateTime.MinValue;
                }

                if (haveLink)
                {
                    label.Text = string.Format(WebTools.BranchLinkStub,
                        branchLink.ToString(),
                        branchName);
                }

                if (haveAddress)
                {
                    label.Text += string.Format(WebTools.BranchMapStub,
                        HttpUtility.UrlEncode(branchAddress.ToString()));
                }

                try
                {
                    if (haveLink && haveAddress && eventDate != DateTime.MinValue)
                    {
                        string detailsLink = string.Format("{0}{1}",
                            WebTools.GetBaseUrl(Request),
                            ResolveUrl(string.Format("~/Events/Details.aspx?EventId={0}", eventRow["EID"])));

                        SchemaOrgLibrary mdLib = new SchemaOrgLibrary
                        {
                            Name = branchName,
                            Address = branchAddress.ToString(),
                            Url = branchLink.ToString()
                        };

                        if (branchTelephone != null && !string.IsNullOrWhiteSpace(branchTelephone.ToString()))
                        {
                            mdLib.Telephone = branchTelephone.ToString();
                        }

                        SchemaOrgEvent mdEvt = new SchemaOrgEvent
                        {
                            Name = eventRow["EventTitle"].ToString(),
                            Url = detailsLink,
                            Location = mdLib,
                            StartDate = eventDate
                        };

                        var md = e.Item.FindControl("Microdata") as Label;

                        if (md != null)
                        {
                            md.Text = new WebTools().BuildEventJsonld(mdEvt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Log().Error("Problem creating microdata in event list for {0}: {1} - {2}",
                        eventRow["EID"],
                        ex.Message,
                        ex.StackTrace);
                }
            }
        }

        protected void UpdateBranchList()
        {
            BranchDataSource.Select();
            BranchId.Items.Clear();
            BranchId.Items.Add(new ListItem("All libraries/branches", "0"));
            BranchId.DataBind();
            if (BranchId.Items.Count == 2)
            {
                BranchId.SelectedIndex = 1;
            }
        }

        protected void SystemId_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateBranchList();
        }

        protected void ShowThisWeek()
        {
            ClearSelections();
            StartDate.Text = DateTime.Now.ToShortDateString();
            EndDate.Text = DateTime.Now.AddDays(7).ToShortDateString();
            DoFilter();
        }

        protected void ThisWeek_Click(object sender, EventArgs e)
        {
            ShowThisWeek();
        }
    }
}
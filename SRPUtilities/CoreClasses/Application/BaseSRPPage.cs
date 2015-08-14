using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.Utilities.CoreClasses;

namespace SRPApp.Classes
{
    public class BaseSRPPage : System.Web.UI.Page
    {
        public BaseSRPPage ()
        {
            PreInit += basePagePreInit;

        }

        #region Properties

        private bool _isSecure = false;
        public bool IsSecure
        {
            get { return _isSecure; }
            set { 
                    _isSecure = value;
                    if (IsSecure && !IsLoggedIn) Response.Redirect("~/Login.aspx"); 
                }
        }

        public int PatronID = 0;
        public int ProgramID = 0;
        //public Patron patron = null;

            public string srpPageName = ""; 
            public int srpPageID = -1;

            //public DataSet pageInfo;
            //public DataSet pageContent;
            //public DataSet pageWidgetSettings;

            public Hashtable PageFlags = new Hashtable();

            protected BaseSRPMaster MasterPage;

        #endregion

        public bool IsLoggedIn = false;
        //{
        //    get
        //    {
                
        //    }
        //}

        protected override void OnPreLoad(EventArgs e)
        {
            MasterPage = (BaseSRPMaster)Master;

            // Tenant not selected ...  
            if (Session["TenantID"] == null || Session["TenantID"].ToString() == "")
            {
                // Direct program ID hit
                if (!String.IsNullOrEmpty(Request["PID"]))
                {
                    // we can get the get the Tenant ID from the Program 
                    // Set the tenant
                    var tenID = Tenant.GetTenantByProgram(Request["PID"].SafeToInt());

                    if (tenID < 0)
                    {
                        Response.Redirect("~/Select.aspx", true); 
                    }
                    Session["TenantID"] = tenID;
                }
                else
                {
                    // Check domain name and see if we can get the tenant from there
                    // Set then tenant
                    var dom = Request.ServerVariables["HTTP_HOST"];
                    var port = Request.ServerVariables["SERVER_PORT"];
                    if (port == "80") port = ""; else port = ":" + port;
                    if (dom.Contains(":"))
                    {
                        dom = dom.Substring(0, dom.IndexOf(":")) + port;
                    }
                    var tenID = -1;
                    try
                    {
                        tenID = Tenant.GetTenantByDomainName(dom);                        
                    }
                    catch
                    {
                        Response.Redirect("~/ControlRoom/Setup.aspx");
                    }
                    // else go to tenant selection page ..
                    if (tenID < 0)
                    {
                        Response.Redirect("~/Select.aspx", true);
                    }
                    Session["TenantID"] = tenID;
                }
            }

            IsLoggedIn = false;
            if (Session["PatronLoggedIn"] != null && (bool)Session["PatronLoggedIn"])
            {
                IsLoggedIn = true;
                // Load the Patron  -- SRPUser = (SRPUser)Session[SessionData.UserProfile.ToString()];
                // PatronID =  patron.PK
                // ProgramID = patron.programid
            }
            if (IsSecure && !IsLoggedIn) Response.Redirect("~/Login.aspx"); 
            
            
            base.OnPreLoad(e);
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnInit(e);
        }


        protected void PageLoad(object sender, EventArgs e)
        {
            srpPageName = (string.IsNullOrEmpty(Request["PageName"]) ? "" : Request["PageName"]);
            //LoadPageInfo();
        }

        //public void LoadPageInfo()
        //{

        //    if (!string.IsNullOrEmpty(srpPageName))
        //    {
        //        //int cmsPageID = CMSPageHelper.FindPageId(srpPageName);

        //        //pageInfo = CMSPageHelper.SelectPageInfo(cmsPageID);
        //        //pageContent = CMSPageHelper.SelectPageContent(cmsPageID);
        //        //pageWidgetSettings = CMSPageHelper.SelectPageWidgetSettings(cmsPageID);
        //    }
        //}

        public void TranslateStrings(Control ctl)
        {
            MasterPage = (BaseSRPMaster)Master;
            MasterPage.InitResFile();

            LoadLabels(ctl);
            LoadValidators(ctl);
            LoadRadioButtonLists(ctl);
            LoadDropDownListLists(ctl);
            LoadButtons(ctl);
        }

        public void TranslateDropDownList(Control c)
        {
            if (c is DropDownList)
            {
                var dd = (DropDownList)c;
                foreach (ListItem i in dd.Items)
                {
                    if (((ListItem)i).Value == ((ListItem)i).Text) ((ListItem)i).Value = ((ListItem)i).Text;
                    ((ListItem)i).Text = GetResourceString(((ListItem)i).Text);
                }
            }
        }
        public void LoadDropDownListLists(Control ctl)
        {
            foreach (Control c in ctl.Controls)
            {
                LoadDropDownListLists(c);
                if (c is DropDownList)
                {
                    var dd = (DropDownList)c;
                    foreach (ListItem i in dd.Items)
                    {
                        if (((ListItem)i).Value == ((ListItem)i).Text) ((ListItem)i).Value = ((ListItem)i).Text;
                        ((ListItem)i).Text = GetResourceString(((ListItem)i).Text);
                    }
                }
            }
        }
        public void TranslateRadioButtonList(Control c)
        {
            if (c is RadioButtonList)
            {
                var rb = (RadioButtonList)c;
                foreach (ListItem i in rb.Items)
                {
                    if (((ListItem)i).Value == ((ListItem)i).Text) ((ListItem)i).Value = ((ListItem)i).Text;
                    ((ListItem)i).Text = GetResourceString(((ListItem)i).Text);
                }
            }
        }
        public void TranslateDdList(Control c)
        {
            if (c is DropDownList)
            {
                var rb = (DropDownList)c;
                foreach (ListItem i in rb.Items)
                {
                    if (((ListItem)i).Value == ((ListItem)i).Text) ((ListItem)i).Value = ((ListItem)i).Text;
                    ((ListItem)i).Text = GetResourceString(((ListItem)i).Text);
                }
            }
        }
        public void LoadRadioButtonLists(Control ctl)
        {
            foreach (Control c in ctl.Controls)
            {
                LoadRadioButtonLists(c);
                if (c is RadioButtonList)
                {
                    var rb = (RadioButtonList)c;
                    foreach (ListItem i in rb.Items)
                    {
                        if (((ListItem)i).Value == ((ListItem)i).Text) ((ListItem)i).Value = ((ListItem)i).Text;
                        ((ListItem)i).Text = GetResourceString(((ListItem)i).Text);
                    }
                }
            }
        }
        public void LoadButtons(Control ctl)
        {
            foreach (Control c in ctl.Controls)
            {
                LoadButtons(c);
                if (c is Button)
                {
                    ((Button)c).Text = GetResourceString(((Button)c).Text);
                }
                if (c is LinkButton)
                {
                    ((LinkButton)c).Text = GetResourceString(((LinkButton)c).Text);
                }
            }
        }
        public void LoadLabels(Control ctl)
        {
            foreach (Control c in ctl.Controls)
            {
                LoadLabels(c);
                if (c is Label)
                {
                    ((Label)c).Text = GetResourceString(((Label)c).Text);
                }
            }
        }
        public void LoadValidators(Control ctl)
        {
            foreach (Control c in ctl.Controls)
            {
                LoadValidators(c);
                if (c is RequiredFieldValidator)
                {
                    ((RequiredFieldValidator)c).ErrorMessage = GetResourceString(((RequiredFieldValidator)c).ErrorMessage);
                }
                if (c is RangeValidator)
                {
                    ((RangeValidator)c).ErrorMessage = GetResourceString(((RangeValidator)c).ErrorMessage);
                }
                if (c is RegularExpressionValidator)
                {
                    ((RegularExpressionValidator)c).ErrorMessage = GetResourceString(((RegularExpressionValidator)c).ErrorMessage);
                }
                if (c is CompareValidator)
                {
                    ((CompareValidator)c).ErrorMessage = GetResourceString(((CompareValidator)c).ErrorMessage);
                }
                if (c is CustomValidator)
                {
                    ((CustomValidator)c).ErrorMessage = GetResourceString(((CustomValidator)c).ErrorMessage);
                }

            }
        }

        //public static PageServices GetUserControlPageService(UserControl control)
        //{
        //    var pageParent = (control.Page as BasePage);
        //    if (pageParent == null)
        //        return new PageServices(control.Page);
        //    return pageParent.PageService;
        //}

        private void basePagePreInit(object sender, EventArgs e)
        {
            try
            {
                MasterPage = (BaseSRPMaster)Master;
                MasterPage.InitResFile();
            }
            catch //(Exception ex)
            {
            }
            if ("y".Equals(Request.QueryString["print"]))
                MasterPageFile = "~/master/Print.master";
        }

        public string GetResourceString(string resName)
        {

            try
            {
                //return MasterPage.rm.GetString(resName);

                return StringResources.getString(resName);
            }
            catch //(Exception ex)
            {
                return resName;
            }
        }

        public Control FindControlRecursive(Control rootControl, string controlID)
        {
            if (rootControl.ID == controlID) return rootControl;

            foreach (Control controlToSearch in rootControl.Controls)
            {
                Control controlToReturn =
                    FindControlRecursive(controlToSearch, controlID);
                if (controlToReturn != null) return controlToReturn;
            }
            return null;
        }

    }
}
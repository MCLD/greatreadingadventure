using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP
{
    public partial class Select : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["TenantID"]= string.Empty;
            // Get master tenant
            // get dafault program for master tenant
            if (!IsPostBack)
            {
                var TenID = Tenant.GetMasterID();

                var PID = Programs.GetDefaultProgramID(TenID);

                DefPID = PID.ToString();

                var t = Tenant.FetchObject(TenID);
                txtMasterDesc.Text = HttpUtility.HtmlDecode(t.Description);

                btnSelProgram.Enabled = false;

                var ds = Tenant.GetAllActive();
                ddTenants.DataValueField = "TenID";
                ddTenants.DataTextField = "LandingName";
                ddTenants.DataSource = ds;
                ddTenants.DataBind();

                Tenant tenant = null;

                if(ddTenants.Items.Count == 1 && string.IsNullOrEmpty(ddTenants.Items[0].Value)) {
                    // if there are no tenants then we'll go directly to the master tenant
                    tenant = Tenant.FetchObject(TenID);
                } else if(ddTenants.Items.Count == 2 && string.IsNullOrEmpty(ddTenants.Items[0].Value)) {
                    // if there is only one tenant we will default to that
                    var row = ds.Tables[0].Rows[0];
                    var tenId = row["TenId"] as int?;
                    if(tenId != null) {
                        tenant = Tenant.FetchObject((int)tenId);
                    }
                }
                if(tenant != null) {
                    Session["TenantID"] = tenant.TenID;
                    Response.Redirect("~/Default.aspx");
                }
            }
            TranslateStrings(this);
        }
        protected void ddTenants_SelectedIndexChanged(object sender, EventArgs e)
        {
            var TenID = ddTenants.SelectedValue;
            if (TenID == "")
            {
                txtTenDesc.Text= string.Empty;
                btnSelProgram.Enabled = false;
            }
            else
            {
                var t = Tenant.FetchObject(TenID.SafeToInt());
                if (t != null)
                {
                    txtTenDesc.Text = HttpUtility.HtmlDecode(t.Description);
                    btnSelProgram.Enabled = true;
                }
                else
                {
                    txtTenDesc.Text= string.Empty;
                    btnSelProgram.Enabled = false;
                }
            }
        }

        protected void btnSelProgram_Click(object sender, EventArgs e)
        {
            var TenID = ddTenants.SelectedValue;
            Session["TenantID"] = TenID.SafeToInt();
            Response.Redirect("~/Default.aspx");
        }

        public string DefPID { get { return (ViewState["PID"] == null ? null : ViewState["PID"].ToString()); } set { ViewState["PID"] = value; } }

        public void TranslateStrings(Control ctl)
        {
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

        public string GetResourceString(string resName)
        {

            try
            {
                return StringResources.getString(DefPID, resName);
            }
            catch //(Exception ex)
            {
                return resName;
            }
        }



    }
}
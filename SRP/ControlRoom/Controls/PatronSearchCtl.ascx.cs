using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;

namespace GRA.SRP.ControlRoom.Controls
{
    public partial class PatronSearchCtl : System.Web.UI.UserControl
    {
        public void ProgID_DataBound(object sender, EventArgs e)
        {
            if (ProgID.Items.Count == 2)
            {
                ProgID.SelectedIndex = 1;
                Session["PS_Prog"] = ProgID.SelectedValue;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                if (LibraryDistrictId.SelectedValue != SelectedDistrictId.Value)
                {
                    SelectedDistrictId.Value = LibraryDistrictId.SelectedValue;
                    LibraryBranchData.DataBind();
                    LibraryId.Items.Clear();
                    LibraryId.Items.Add(new ListItem("[Select a Library Branch]", string.Empty));
                    LibraryId.DataBind();
                }
            }
        }

        public void SetFilterSessionValues()
        {
            Session["PS_First"] = txtFirstName.Text.Replace('*', '%');
            Session["PS_Last"] = txtLastName.Text.Replace('*', '%');
            Session["PS_User"] = txtUsername.Text.Replace('*', '%');
            Session["PS_Email"] = txtEmail.Text.Replace('*', '%');
            Session["PS_DOB"] = txtDOB.Text;
            Session["PS_Prog"] = ProgID.SelectedValue;
            Session["PS_Gender"] = Gender.SelectedValue;
            Session["PS_LibraryDistrictId"] = LibraryDistrictId.SelectedValue;
            Session["PS_LibraryId"] = LibraryId.SelectedValue;

            Session["PS_Filtered"] = "1";
        }

        public void GetFilterSessionValues()
        {
            if (Session["PS_First"] != null) txtFirstName.Text = Session["PS_First"].ToString().Replace('%', '*');
            if (Session["PS_Last"] != null) txtLastName.Text = Session["PS_Last"].ToString().Replace('%', '*');
            if (Session["PS_User"] != null) txtUsername.Text = Session["PS_User"].ToString().Replace('%', '*');
            if (Session["PS_Email"] != null) txtEmail.Text = Session["PS_Email"].ToString().Replace('%', '*');
            if (Session["PS_DOB"] != null) txtDOB.Text = Session["PS_DOB"].ToString();
            if (Session["PS_Prog"] != null) ProgID.SelectedValue = Session["PS_Prog"].ToString();
            if (Session["PS_Gender"] != null)
            {
                string selectedGender = Session["PS_Gender"].ToString();
                if (Gender.Items.FindByValue(selectedGender) != null)
                {
                    Gender.SelectedValue = selectedGender;
                }
            }
            else
            {
                Gender.SelectedIndex = 0;
            }
            if (Session["PS_LibraryDistrictId"] != null)
            {
                string selectedDistrict = Session["PS_LibraryDistrictId"].ToString();
                if (LibraryDistrictId.Items.FindByValue(selectedDistrict) != null)
                {
                    LibraryDistrictId.SelectedValue = selectedDistrict;
                }
            }
            else
            {
                LibraryDistrictId.SelectedIndex = 0;
            }
            if (Session["PS_LibraryId"] != null)
            {
                string selectedLibrary = Session["PS_LibraryId"].ToString();
                if (LibraryId.Items.FindByValue(selectedLibrary) != null)
                {
                    LibraryId.SelectedValue = selectedLibrary;
                }
            }
            else
            {
                LibraryId.SelectedIndex = 0;
            }
        }

        public bool WasFiltered()
        {
            return (Session["PS_Filtered"] != null);
        }

        public DataSet GetFilteredList()
        {
            SetFilterSessionValues();
            return
                Patron.CRSearch();

        }
    }
}
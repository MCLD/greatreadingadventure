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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !WasFiltered())
            {
                LoadDropdowns();
            }
        }

        public void LoadDropdowns()
        {
            //// Load Agency Dropdown;
            //DataSet dsAgency = DAL.Filters.SelectAllAgencies();
            //dsAgency.Tables[0].Rows.InsertAt(dsAgency.Tables[0].NewRow(), 0);
            //dsAgency.Tables[0].Rows[0]["Name"]= string.Empty;
            //ddlAgencies.DataSource = dsAgency;
            //ddlAgencies.DataTextField = "Name";
            //ddlAgencies.DataValueField = "UID";
            //ddlAgencies.DataBind();


            //// Load Program
            //DataSet dsPrograms = DAL.Filters.SelectAllPrograms();
            //dsPrograms.Tables[0].Rows.InsertAt(dsPrograms.Tables[0].NewRow(), 0);
            //dsPrograms.Tables[0].Rows[0]["Name"]= string.Empty;
            //ddlPrograms.DataSource = dsPrograms;
            //ddlPrograms.DataTextField = "Name";
            //ddlPrograms.DataValueField = "UID";
            //ddlPrograms.DataBind();


            //// Load Year
            //DataSet dsYears = DAL.Filters.SelectAllRecordFiscalYears();
            //dsYears.Tables[0].Rows.InsertAt(dsYears.Tables[0].NewRow(), 0);
            //dsYears.Tables[0].Rows[0]["Name"]= string.Empty;
            //ddlYears.DataSource = dsYears;
            //ddlYears.DataTextField = "Name";
            //ddlYears.DataValueField = "Value";
            //ddlYears.DataBind();
        }

        public void SetFilterSessionValues()
        {
            Session["PS_First"] = txtFirstName.Text;
            Session["PS_Last"] = txtLastName.Text;
            Session["PS_User"] = txtUsername.Text;
            Session["PS_Email"] = txtEmail.Text;
            Session["PS_DOB"] = txtDOB.Text;
            Session["PS_Prog"] = ProgID.SelectedValue;
            Session["PS_Gender"] = Gender.SelectedValue;

            Session["PS_Filtered"] = "1";
        }

        public void GetFilterSessionValues()
        {
            if (Session["PS_First"] != null) txtFirstName.Text = Session["PS_First"].ToString();
            if (Session["PS_Last"] != null) txtLastName.Text = Session["PS_Last"].ToString();
            if (Session["PS_User"] != null) txtUsername.Text = Session["PS_User"].ToString();
            if (Session["PS_Email"] != null) txtEmail.Text = Session["PS_Email"].ToString();
            if (Session["PS_DOB"] != null) txtDOB.Text = Session["PS_DOB"].ToString();
            if (Session["PS_Prog"] != null) ProgID.SelectedValue = Session["PS_Prog"].ToString();
            if (Session["PS_Gender"] != null) Gender.SelectedValue = Session["PS_Gender"].ToString();
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
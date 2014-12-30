using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using STG.SRP.DAL;
using STG.SRP.Utilities.CoreClasses;


namespace STG.SRP.Controls
{
    public partial class Events : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                GetFilterSessionValues();
                GetData();

            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            SetFilterSessionValues();
            GetData();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            StartDate.Text = EndDate.Text = "";
            BranchId.SelectedValue = "0";
            SetFilterSessionValues();
            GetData();
        }

        public void GetData()
        {
            var ds = DAL.Event.GetUpcomingDisplay(Session["UEL_Start"].ToString(), Session["UEL_End"].ToString(), int.Parse(Session["UEL_Branch"].ToString()));
            rptr.DataSource = ds;
            rptr.DataBind();
            ((BaseSRPPage)Page).TranslateStrings(rptr);            
        }
        public void SetFilterSessionValues()
        {
            Session["UEL_Start"] = StartDate.Text;
            Session["UEL_End"] = EndDate.Text;
            Session["UEL_Branch"] = BranchId.SelectedValue;
            Session["UEL_Filtered"] = "1";
        }

        public void GetFilterSessionValues()
        {
            if (Session["UEL_Start"] != null) 
            {
                StartDate.Text = Session["UEL_Start"].ToString();
            }
            else
            {
                Session["UEL_Start"] = "";
            }
            if (Session["UEL_End"] != null)
            {
                EndDate.Text = Session["UEL_End"].ToString();
            }
            else
            {
                Session["UEL_End"] = "";
            }
            if (Session["UEL_Branch"] != null) 
            {
                try { BranchId.SelectedValue = Session["UEL_Branch"].ToString(); }
                catch (Exception) { }
            }
            else
            {
                Session["UEL_Branch"] = 0;
            }
        }

        public bool WasFiltered()
        {
            return (Session["UEL_Filtered"] != null);
        }



        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            pnlList.Visible = false;


            var o = new Event();
            o.Fetch(int.Parse(e.CommandArgument.ToString()));

            lblTitle.Text = o.EventTitle;
            lblWhen.Text = FormatHelper.ToNormalDate(o.EventDate) + " " + o.EventTime;
            var c = new Codes();
            if (o.BranchID != 0) lblWhere.Text = (c.FetchObject(o.BranchID)).Code;

            lblHtml.Text = o.HTML;

            var cf = CustomEventFields.FetchObject();
            if (cf.Use1)
            {
                Panel1.Visible = true;
                CF1Label.Text = cf.Label1;
                CF1Value.Text = o.Custom1;
            }
            if (cf.Use2)
            {
                Panel2.Visible = true;
                CF2Label.Text = cf.Label2;
                CF2Value.Text = o.Custom2;
            } 
            if (cf.Use3)
            {
                Panel3.Visible = true;
                CF3Label.Text = cf.Label3;
                CF3Value.Text = o.Custom3;
            }
            pnlDetail.Visible = true;
        }

        protected void btnList_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Events.aspx");
        }
    }
}
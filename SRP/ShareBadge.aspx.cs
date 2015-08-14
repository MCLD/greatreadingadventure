using System;
using SRPApp.Classes;
using GRA.SRP.DAL;

namespace GRA.SRP
{
    public partial class ShareBadge : BaseSRPPage //System.Web.UI.Page
    {

        protected override void OnPreLoad(EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request["PID"]))
            {
                Session["ProgramID"] = Request["PID"].ToString();
            }
            if (!IsPostBack)
            {

                if (Session["ProgramID"] == null)
                {
                    var BID = 0;
                    if (Request["BID"] != null && Request["BID"] != "")
                    {
                        int.TryParse(Request["BID"].ToString(), out BID);
                    }
                    var b = new Badge();
                    b.Fetch(BID);
                    if (b.BID > 0)
                    {
                        Session["TenantID"] = b.TenID;
                        try
                        {
                            int PID = Programs.GetDefaultProgramID();
                            Session["ProgramID"] = PID.ToString();
                        }
                        catch
                        {
                            Response.Redirect("~/Select.aspx");
                        }

                    }
                    try
                    {
                        int PID = Programs.GetDefaultProgramID();
                        Session["ProgramID"] = PID.ToString();
                    }
                    catch
                    {
                        Response.Redirect("~/Select.aspx");
                    }
                    // pgmID.Text = Session["ProgramID"].ToString();
                }
                else
                {
                    //pgmID.Text = Session["ProgramID"].ToString();
                }


            }
            TranslateStrings(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /*
             * if (!String.IsNullOrEmpty(Request["PID"]))
            {
                Session["ProgramID"] = Request["PID"].ToString();
            }
            if (!IsPostBack)
            {

                if (Session["ProgramID"] == null)
                {
                    var BID = 0;
                    if (Request["BID"] != null && Request["BID"] != "")
                    {
                        int.TryParse(Request["BID"].ToString(), out BID); 
                    }
                    var b = new Badge();
                    b.Fetch(BID);
                    if (b.BID > 0)
                    {
                        Session["TenantID"] = b.TenID;
                        try
                        {
                            int PID = Programs.GetDefaultProgramID();
                            Session["ProgramID"] = PID.ToString();
                        }
                        catch
                        {
                            Response.Redirect("~/Select.aspx");
                        }

                    }
                    try
                    {
                        int PID = Programs.GetDefaultProgramID();
                        Session["ProgramID"] = PID.ToString();
                    }
                    catch
                    {
                        Response.Redirect("~/Select.aspx");
                    }
                    // pgmID.Text = Session["ProgramID"].ToString();
                }
                else
                {
                    //pgmID.Text = Session["ProgramID"].ToString();
                }


            }*/
            TranslateStrings(this);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //Session["ProgramID"] = pgmID.Text;
            //TranslateStrings(this);
            Response.Redirect("/");
        }
    }
}

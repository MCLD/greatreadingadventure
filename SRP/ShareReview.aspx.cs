using System;
using SRPApp.Classes;
using GRA.SRP.DAL;

namespace GRA.SRP
{
    public partial class ShareReview : BaseSRPPage
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
                    var PRID = 0;
                    if (Request["ID"] != null && Request["ID"] != "")
                    {
                        int.TryParse(Request["ID"].ToString(), out PRID);
                    }
                    var pr = new PatronReview();
                    pr.Fetch(PRID);
                    if (pr.PID <= 0)
                    {
                        Response.Redirect("~/Select.aspx");
                    }
                    var p = Patron.FetchObject(pr.PID);

                    Session["TenantID"] = p.TenID;
                    Session["ProgramID"] = p.ProgID;

                    if (p.ProgID <= 0)
                    {
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
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            TranslateStrings(this);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("/");
        }

        public string FormatReading(string author, string title, string review)
        {
            var ret = "";

            if (author != "" && title != "")
            {
                ret = string.Format("<h2><b>{0}</b> by <i>{1}</i></h2>", title, author);
            }
            if (author != "" && title == "")
            {
                ret = string.Format("<h2>A book by <i>{0}</i></h2>", author);
            }
            if (author == "" && title != "")
            {
                ret = string.Format("<h2><b>{0}</b></h2>", title);
            }
            if (review.Trim() != "")
            {
                ret = string.Format("{0}<br/><br/>{1}", ret, review);
            }
            return ret;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;

namespace GRA.SRP
{
    public partial class FramedSurvey : BaseSRPPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if ((Request["SID"] == null || Request["SID"] == ""|| Request["SSrc"] == null || Request["SSrc"] == ""||Request["SSrcID"] == null || Request["SSrcID"] == "")
                        && (Request["C"] == null || Request["C"] == "") )
                {
                    Response.Redirect("Blank.aspx");
                    return;
                }

                if (Request["C"] == null || Request["C"] == "")
                {
                    Session["SRID"] = 0;            // which results to continue
                    Session["SID"] = int.Parse(Request["SID"]);      // the test to restart 
                    Session["QNum"] = 0;  // question to restart from
                    Session["SSrc"] = Survey.Source(int.Parse(Request["SSrc"])); // pre - testing
                    Session["SSrcID"] = int.Parse(Request["SSrcID"]);         // program id
                    Session["Page"] = 1;
                }

            }


            // FramedSurvey.aspx?SID=1&SSrc=1&SSrcID=2
        }
    }
}
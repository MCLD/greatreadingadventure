using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Modules.Patrons
{
    public partial class PatronSurveyDetails : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5100;
            MasterPage.IsSecure = true;
            if (Session["Curr_Patron"] == null) Response.Redirect("Default.aspx");

            if (!IsPostBack)
            {
                PatronsRibbon.GetByAppContext(this);
            }

            if (!IsPostBack)
            {
                var patron = (Patron)Session["Curr_Patron"];
                MasterPage.PageTitle = string.Format("{0} - {1} {2} ({3})", "Patron Test/Survey Detail", patron.FirstName, patron.LastName, patron.Username);

                LoadData();

            }

        }

        protected void LoadData()
        {
            var patron = (Patron)Session["Curr_Patron"];
            if (Session["PSRID"] != null) SRID.Text = Session["PSRID"].ToString();
            if (Session["PSRID"] == null  && SRID.Text == "") Response.Redirect("PatronSurveys.aspx");
            Session["PSRID"] = null;


            var sr = SurveyResults.FetchObject(int.Parse(SRID.Text));
            var s = Survey.FetchObject(sr.SID);

            Name.Text = string.Format("{0} ({1})", s.Name, s.LongName);
            C.Text = string.Format("{0}", sr.IsComplete.ToYesNo());
            S.Text = string.Format("{0}", sr.IsComplete && sr.IsScorable ? sr.Score.ToString() : "");
            P.Text = string.Format("{0}", sr.IsComplete && sr.IsScorable ? sr.ScorePct.ToString() : "");
            D.Text = string.Format("{0}", sr.IsComplete && sr.IsScorable ? sr.EndDate.ToWidgetDisplayDate() : "");


            var ds = DAL.SurveyAnswers.GetAllExpanded(sr.SRID);

            rptr.DataSource = ds;
            rptr.DataBind();
        }

        public string DisplayAnswers(int QType, int QID, int SQMLID, string ChoiceAnswerIDs, string ChoiceAnswerText, string ClarificationText, bool MXShowChoices)
        {
            var displayString= string.Empty;

            if (QType == 2) // Multiple Choice
            {
                string[] separators = {"~|~"};
                var mcIDs = ChoiceAnswerIDs.Split(',');
                var mcTXs = ChoiceAnswerIDs.Split(separators, StringSplitOptions.None);
                var mcCLs = ClarificationText.Split(separators, StringSplitOptions.None);

                var ds4 = SQChoices.GetAll(QID);
                foreach (DataRow dr4 in ds4.Tables[0].Rows)
                {
                    var idx4 = -1;
                    var match = FindAnswer(ChoiceAnswerIDs, mcIDs, (int) dr4["SQCID"], out idx4);
                    var ansString = string.Format("<tr><td valign='top'><b>{2}</b></td><td valign='top' nowrap>{0}</td><td nowrap style='padding-right: 5px;' align='right' valign='top'>({1} pts)</td><td style='padding-left:10px;' valign='top'>{3}</td></tr>"
                                                  , dr4["ChoiceText"].ToString(), ((int) dr4["Score"])
                                                  , match ? " X " : ""
                                                  , match ? mcCLs[idx4] : ""

                                              );
                    displayString = string.Format("{0}{1}", displayString, ansString);
                }

                displayString = string.Format("<table cellpadding=5> {0} </table>", displayString);
            }

            if (QType == 4) // Matrix
            {
                string[] separators = { "~|~" };
                var mcIDs = ChoiceAnswerIDs.Split(',');
                var mcTXs = ChoiceAnswerIDs.Split(separators, StringSplitOptions.None);
                var mcCLs = ClarificationText.Split(separators, StringSplitOptions.None);


                var ds4 = SQChoices.GetAll(QID);
                var ml = SQMatrixLines.FetchObject(SQMLID);

                displayString = string.Format("<tr><td valign='top' style='padding-right: 5px;' width='50%'>{0}</td>", ml.LineText);
                var w = (int) (50/ds4.Tables[0].Rows.Count);

                if (MXShowChoices)
                {

                    foreach (DataRow dr4 in ds4.Tables[0].Rows)
                    {
                        var idx4 = -1;
                        var match = FindAnswer(ChoiceAnswerIDs, mcIDs, (int)dr4["SQCID"], out idx4);
                        var ansString = string.Format("<td valign='top' align='center' width='{4}%' >{0} ({1} pts) <br> <b>{2}</b> <br> <div style='text-align: left;'>{3}</div></td>"
                                                      , dr4["ChoiceText"].ToString()
                                                      , ((int)dr4["Score"])
                                                      , match ? " X " : ""
                                                      , match ? mcCLs[idx4] : ""
                                                      , w
                                                  );
                        displayString = string.Format("{0}{1}", displayString, ansString);
                    }
                }
                else
                {

                    foreach (DataRow dr4 in ds4.Tables[0].Rows)
                    {
                        var idx4 = -1;
                        var match = FindAnswer(ChoiceAnswerIDs, mcIDs, (int)dr4["SQCID"], out idx4);
                        var ansString = string.Format("<td valign='top' align='center' width='{2}%' ><b>{0}</b> <br> <div style='text-align: left;'>{1}</div></td>"
                                                      , match ? " X " : ""
                                                      , match ? mcCLs[idx4] : ""
                                                      , w
                                                  );
                        displayString = string.Format("{0}{1}", displayString, ansString);
                    }
                }

                displayString = string.Format("<table cellpadding=5 width='100%'> {0} </tr></table>", displayString);
            }

            return displayString;
        }


        public bool FindAnswer(string ChoiceAnswerIDs, string[] IDs, int ID, out int index)
        {
            if (ChoiceAnswerIDs.IndexOf(ID.ToString()) < 0)
            {
                index = -1;
                return false;
                
            }

            for (int i = 0; i<IDs.Length ;i++)
            {
                if (IDs[i] != ID.ToString()) continue;
                index = i;
                return true;
            }
            index = -1;
            return false;
        }
    
    }
}
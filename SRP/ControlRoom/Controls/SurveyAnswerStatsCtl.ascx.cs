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
    public partial class SurveyAnswerStatsCtl : System.Web.UI.UserControl
    {
        private int qid;
        private int sid;
        private int sqmlid;
        private int qtype;
        private int qnumber;
        private int showqtext;
        private bool graphs;

        private string SourceType;
        private int SourceID;

        public string Source
        {
            set
            {
                if (value=="0")
                {
                    SourceType= string.Empty;
                    SourceID = 0;
                }
                else
                {
                    string[] stringSeparators = new string[] { "_|_" };
                    var arr = value.Split(stringSeparators, StringSplitOptions.None);
                    SourceType = arr[0];
                    SourceID = int.Parse(arr[1]);
                }
            }
        }


        public int QID
        {
            get { return qid; }
            set { qid = value; }
        }

        public int SID
        {
            get { return sid; }
            set { sid = value; }
        }

        public int SQMLID
        {
            get { return sqmlid; }
            set { sqmlid = value; }
        }

        public int QType
        {
            get { return qtype; }
            set { qtype = value; }
        }

        public int QNumber
        {
            get { return qnumber; }
            set { qnumber = value; }
        }

        public int ShowQText
        {
            get { return showqtext; }
            set { showqtext = value; }
        }

        public bool Graphs
        {
            get { return graphs; }
            set { graphs = value; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public string LoadQStats()
        {
            var q = DAL.SurveyQuestion.FetchObject(QID);

            var line1 = "<tr><td colspan=3>{0}</td></tr>";
            var line2 =
                "<tr><td style='padding-left: 10px;' valign=top align=left  width='33%'><b>Minimum Details: </b></td>"+
                "<td style='padding-right: 50px;' valign=top align=left  width='33%'><b>Medium Details</b> <a style='float:right;' class='ml{3}' href='javascript: showhide(\"ml{3}\", \"mt{3}\")'>show</a></td>" +
                "<td style='padding-right: 50px;' valign=top align=left  width='33%'><b>Full Details</b> <a style='float:right;' class='fl{3}' href='javascript: showhide(\"fl{3}\", \"ft{3}\")'>show</a></td></tr>" +
                "<tr><td style='padding-left: 10px;'valign=top align=left>{0}</td><td valign=top align=left>{1}</td><td>{2}</td></tr>";
            var line3 =
                "<tr><td style='padding-left: 10px;'valign=top align=left colspan=3>{0}</td></tr>";
            var line4 = "<tr><td colspan=3><hr></td></tr>";

            var line5 = "<tr><td style='padding-left: 10px;' valign=top align=left colspan=3>{0}</td></tr>";
            var ret= string.Empty;
            
            if (QType == 2) // multiple choice
            {
                ret = string.Format(line1, q.QText);
                var ds = SurveyResults.GetQStatsSimple(SID, QID, SQMLID, SourceType, SourceID);
                var header = "<tr><td ><b>Answer</b></td><td width='100px' align=right><b>Count</td></tr>";
                var data = "<tr><td>{0}</td><td align=right>{1}</td></tr>";
                var min= string.Empty;
                var grDataTmplt = "['{0}', {1}],";
                var grData= string.Empty;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    min = string.Format("{0}{1}{2}", min, "\n", string.Format(data, row["ChoiceText"], row["Count"]));
                    if (Graphs) grData = string.Format("{0}{1}{2}", grData, "\n", string.Format(grDataTmplt, row["ChoiceText"], row["Count"]));
                }
                if (grData.Length > 0)
                {
                    grData = grData.Substring(0, grData.Length - 1);
                    min = min + getGraph("_s", QID, SQMLID, grData);
                }

                ds = SurveyResults.GetQStatsMedium(SID, QID, SQMLID, SourceType, SourceID);
                var med= string.Empty;
                grData= string.Empty;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    med = string.Format("{0}{1}{2}", med, "\n", string.Format(data, row["ChoiceText"], row["Count"]));
                    if (Graphs) grData = string.Format("{0}{1}{2}", grData, "\n", string.Format(grDataTmplt, row["ChoiceText"], row["Count"]));
                }
                if (grData.Length > 0)
                {
                    grData = grData.Substring(0, grData.Length - 1);
                    med = med + getGraph("_m", QID, SQMLID, grData);
                }

                //var header2 = "<tr><td><b>Answer AND Clarifications</b></td><td  width='100px' align=right><b>Count</td>";
                var data2 = "<tr><td valigh=top>{0}</td><td align=right valigh=top>{1}</td></tr>";
                var data3 = "<tr><td valigh=top colspan=2 style='padding-left:20px;'>{0}</td></tr>";
                var max= string.Empty;
                grData= string.Empty;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var ds2 = SurveyResults.GetQClarifications(SID, QID, SQMLID, SourceType, SourceID, row["ChoiceTextORIGINAL"].ToString());

                    var clText= string.Empty;
                    foreach (DataRow cl in ds2.Tables[0].Rows)
                    {
                        clText = clText + (clText.Length == 0 ? "" : "<br>") + cl["ClarificationText"].ToString();
                    }

                    max = string.Format("{0}{1}{2}", max, "\n", string.Format(data2, row["ChoiceText"], row["Count"]));
                    if (clText != "")
                    {
                        max = string.Format("{0}{1}{2}", max, "\n", string.Format(data3, clText));
                    }

                    if (Graphs) grData = string.Format("{0}{1}{2}", grData, "\n", string.Format(grDataTmplt, row["ChoiceText"], row["Count"]));
                }
                if (grData.Length > 0)
                {
                    grData = grData.Substring(0, grData.Length - 1);
                    max = max + getGraph("_f", QID, SQMLID, grData);
                }

                line2 = string.Format(line2, "<table>" + header + min + "</table>", 
                                "<table id='mt" + QID + "' style='display:none;'>" + header + med + "</table>",
                                "<table id='ft" + QID + "' style='display:none;'>" + header + max + "</table>", QID);
                ret = ret + line2;
            }
            if (QType == 3) //free form
            {
                ret = string.Format(line1, q.QText);

                var ds2 = SurveyResults.GetQFreeForm(SID, QID, SQMLID, SourceType, SourceID);

                var clText= string.Empty;
                foreach (DataRow cl in ds2.Tables[0].Rows)
                {
                    clText = clText + (clText.Length == 0 ? "" : "<br>") + cl["FreeFormAnswer"].ToString();
                }
                line3 = string.Format(line2, clText);
                ret = ret + line3;

            }

            var line6 =
                "<tr><td style='padding-left: 20px;' valign=top align=left  width='33%'><b>Minimum Details: </b></td>" +
                "<td style='padding-right: 50px;' valign=top align=left  width='33%'><b>Medium Details</b> <a style='float:right;' class='ml{3}_{4}' href='javascript: showhide(\"ml{3}_{4}\", \"mt{3}_{4}\")'>show</a></td>" +
                "<td style='padding-right: 50px;' valign=top align=left  width='33%'><b>Full Details</b> <a style='float:right;' class='fl{3}_{4}' href='javascript: showhide(\"fl{3}_{4}\", \"ft{3}_{4}\")'>show</a></td></tr>" +
                "<tr><td style='padding-left: 20px;'valign=top align=left>{0}</td><td valign=top align=left>{1}</td><td>{2}</td></tr>";

            if (QType == 4) //matrix
            {
                if (ShowQText == 1) ret = string.Format(line1, q.QText);

                var ln = SQMatrixLines.FetchObject(SQMLID);
                ret = ret + string.Format(line5,ln.LineText);
                var grDataTmplt = "['{0}', {1}],";
                var grData= string.Empty;

                var ds = SurveyResults.GetQStatsSimple(SID, QID, SQMLID, SourceType, SourceID);
                var header = "<tr><td ><b>Answer</b></td><td width='100px' align=right><b>Count</td></tr>";
                var data = "<tr><td>{0}</td><td align=right>{1}</td></tr>";
                var min= string.Empty;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    min = string.Format("{0}{1}{2}", min, "\n", string.Format(data, row["ChoiceText"], row["Count"]));
                    if (Graphs) grData = string.Format("{0}{1}{2}", grData, "\n", string.Format(grDataTmplt, row["ChoiceText"], row["Count"]));
                }
                if (grData.Length > 0)
                {
                    grData = grData.Substring(0, grData.Length - 1);
                    min = min + getGraph("_s", QID, SQMLID, grData);
                }

                ds = SurveyResults.GetQStatsMedium(SID, QID, SQMLID, SourceType, SourceID);
                var med= string.Empty;
                grData= string.Empty;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    med = string.Format("{0}{1}{2}", med, "\n", string.Format(data, row["ChoiceText"], row["Count"]));
                    if (Graphs) grData = string.Format("{0}{1}{2}", grData, "\n", string.Format(grDataTmplt, row["ChoiceText"], row["Count"]));
                }
                if (grData.Length > 0)
                {
                    grData = grData.Substring(0, grData.Length - 1);
                    med = med + getGraph("_m", QID, SQMLID, grData);
                }

                //var header2 = "<tr><td><b>Answer AND Clarifications</b></td><td  width='100px' align=right><b>Count</td>";
                var data2 = "<tr><td valigh=top>{0}</td><td align=right valigh=top>{1}</td></tr>";
                var data3 = "<tr><td valigh=top colspan=2 style='padding-left:20px;'>{0}</td></tr>";
                var max= string.Empty;
                grData= string.Empty;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var ds2 = SurveyResults.GetQClarifications(SID, QID, SQMLID, SourceType, SourceID, row["ChoiceTextORIGINAL"].ToString());

                    var clText= string.Empty;
                    foreach (DataRow cl in ds2.Tables[0].Rows)
                    {
                        clText = clText + (clText.Length == 0 ? "" : "<br>") + cl["ClarificationText"].ToString();
                    }

                    max = string.Format("{0}{1}{2}", max, "\n", string.Format(data2, row["ChoiceText"], row["Count"]));
                    if (clText != "")
                    {
                        max = string.Format("{0}{1}{2}", max, "\n", string.Format(data3, clText));
                    }
                    if (Graphs) grData = string.Format("{0}{1}{2}", grData, "\n", string.Format(grDataTmplt, row["ChoiceText"], row["Count"]));
                }
                if (grData.Length > 0)
                {
                    grData = grData.Substring(0, grData.Length - 1);
                    max = max + getGraph("_f", QID, SQMLID, grData);
                }

                line6 = string.Format(line6, "<table>" + header + min + "</table>",
                                "<table id='mt" + QID + "_"+ SQMLID + "' style='display:none;'>" + header + med + "</table>",
                                "<table id='ft" + QID + "_" + SQMLID + "' style='display:none;'>" + header + max + "</table>", QID, SQMLID);
                ret = ret + line6;



                //ret = ret + string.Format(line5, "<hr>");
            }
            return ret + line4;
        }

        private string getGraph(string lvl, int QID, int SQMLID, string data)
        {
            var html = string.Format("<div id='Epie_{0}_{1}_{2}'></div>", lvl,QID,SQMLID);
            var func = @"
                        function EdrawChart{lvl}_{qid}_{sqmlid}() {
                            var data = google.visualization.arrayToDataTable([
                                            ['', ''],
                                            {data}
                                        ]);

                            var options = {
                                width: '320',
                                height: '320',
                                backgroundColor: 'transparent',
                                tooltip: {
                                    textStyle: {
                                        color: '#666666',
                                        fontSize: 11
                                    },
                                    showColorCode: true
                                },
                                legend: {
                                    position: 'right',
                                    textStyle: {
                                        color: 'black',
                                        fontSize: 12
                                    }
                                }
                            };

                            var chart = new google.visualization.PieChart(document.getElementById('Epie_{lvl}_{qid}_{sqmlid}'));
                            chart.draw(data, options);
                        }

                        EdrawChart{lvl}_{qid}_{sqmlid}();

                        ";

            var js= string.Empty;
            js = js + func.Replace("{lvl}", lvl).Replace("{qid}", QID.ToString())
                                    .Replace("{sqmlid}", SQMLID.ToString())
                                    .Replace("{data}", data);
               
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "temp"+lvl+QID.ToString()+"_"+SQMLID.ToString(), "<script type='text/javascript'>" + js + "</script>", false);
            return string.Format("<tr><td colspan=2 align=center>{0}</td></tr>",html);
        }

    }
}
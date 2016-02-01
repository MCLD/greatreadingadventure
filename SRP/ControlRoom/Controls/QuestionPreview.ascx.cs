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
    public partial class QuestionPreview : System.Web.UI.UserControl
    {
        public string QID
        {
            get { return lblQID.Text; }
            set { lblQID.Text = value;}
        }

        public string SID
        {
            get { return lblSID.Text; }
            set { lblSID.Text = value; }
        }

        public string QNumber
        {
            get { return lblQNumber.Text; }
            set { lblQNumber.Text = value; }
        }

        public string QType
        {
            get { return lblQType.Text; }
            set { lblQType.Text = value; }
        }

        public string QText
        {
            get { return lblQText.Text; }
            set { lblQText.Text = value; }
        }

        public string DisplayControl
        {
            get { return lblDisplayControl.Text; }
            set { lblDisplayControl.Text = value; }
        }

        public string DisplayDirection
        {
            get { return lblDisplayDirection.Text; }
            set { lblDisplayDirection.Text = value; }
        }

        public string IsRequired
        {
            get { return lblIsRequired.Text; }
            set { lblIsRequired.Text = value; }
        }

        public string SRID
        {
            get { return lblSRID.Text; }
            set { lblSRID.Text = value; }
        }


        //--//\\----//\\----//\\----//\\----//\\----//\\----//\\----//\\----//\\----//\\----//\\----//\\----//\\--\\


        protected void Page_Load(object sender, EventArgs e)
        {
            if(! IsPostBack) RenderQuestion();
        }


        public void RenderQuestion()
        {
 
            int qtype = 0;
            int.TryParse(lblQType.Text, out qtype);

            Description.Visible = FreeForm.Visible = MultipleChoice.Visible = Matrix.Visible = EndOfPage.Visible = EndOfTest.Visible = false;
            switch (qtype)
            {
                case 1:
                    Description.Visible = true;
                    break;
                case 3:
                    FreeForm.Visible = Description.Visible = true;
                    rfvFreeForm.Enabled = (IsRequired == "yes");
                    break;
                case 2:
                    MultipleChoice.Visible = Description.Visible = true;
                    RenderAnswers();
                    break;
                case 4:
                    Matrix.Visible = Description.Visible = true;
                    break;
                case 5:
                    EndOfPage.Visible = true;
                    break;
                case 6:
                    EndOfTest.Visible = true;
                    break;
            }
        }

        public void RenderAnswers()
        {
            var ds = SQChoices.GetAll(int.Parse(QID));
            if (DisplayControl == "3") //drop down
            {
                ddMultipleChoice.Visible = true;
                ddMultipleChoice.Items.Clear();
                ddMultipleChoice.Items.Add(new ListItem("[ Make a selection ]", "0"));
                foreach (DataRow r  in ds.Tables[0].Rows)
                {
                    ddMultipleChoice.Items.Add(new ListItem(Convert.ToString(r["ChoiceText"]), r["SQCID"].ToString())); 
                }
                rfvddMultipleChoice.Enabled = (IsRequired == "yes");
            }

            if (DisplayControl == "1") //check 
            {
                rptrChk.DataSource = ds;
                rptrChk.DataBind();
                CheckBoxScripts();

            }//

            if (DisplayControl == "2") //radio
            {
                rptrRadio.DataSource = ds;
                rptrRadio.DataBind();
                if (DisplayDirection != "1") RadioBoxScripts();
                
            }//

        }

        protected void ddMultipleChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            var SQCID = int.Parse(ddMultipleChoice.SelectedValue);
            if (SQCID == 0)
            {
                txtDDClarification.Visible = false;
                rfvDDClarification.Enabled = false;
            }
            else
            {
                var a = SQChoices.FetchObject(SQCID);
                txtDDClarification.Visible = a.AskClarification;
                rfvDDClarification.Enabled = a.AskClarification && a.ClarificationRequired;
            }
        }

        protected void chkAns_CheckedChanged(object sender, EventArgs e)
        {
            var chk = ((CheckBox) sender);
            var item = ((CheckBox)sender).Parent as RepeaterItem;
            var lbl = (Label)item.FindControl("SQCID");
            var rfv = (RequiredFieldValidator)item.FindControl("rfvChkClarification");
            var txt = (TextBox)item.FindControl("txtChkClarification");
            
            var SQCID = int.Parse(lbl.Text);
            var a = SQChoices.FetchObject(SQCID);
            if (!chk.Checked)
            {
                rfv.Enabled = false;
                txt.Text= string.Empty;
            } else if (a.AskClarification && a.ClarificationRequired)
            {
                rfv.Enabled = true;
            }
            else
            {
                rfv.Enabled = false;
                txt.Text= string.Empty;
            }

            CheckBoxScripts();
        }

        protected void rbAns_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (RadioButton)sender;
            var item = ((CheckBox)sender).Parent as RepeaterItem;
            var lbl = (Label)item.FindControl("SQCID");


            var SQCID = int.Parse(lbl.Text);
            var a = SQChoices.FetchObject(SQCID);
            
            foreach (RepeaterItem i in rptrRadio.Items)
            {
                var tmpSQCID = int.Parse(((Label)i.FindControl("SQCID")).Text);
                var rfv = (RequiredFieldValidator)i.FindControl("rfvRBClarification");
                var txt = (TextBox)i.FindControl("txtRBClarification");

                if (tmpSQCID != SQCID)
                {
                    rfv.Enabled = false;
                    txt.Text= string.Empty;
                }
                else
                {
                    if (a.AskClarification && a.ClarificationRequired)
                    {
                        rfv.Enabled = true;
                    }
                }
            }

            if (DisplayDirection != "1") RadioBoxScripts();

        }

        public void CheckBoxScripts()
        {
            var script= string.Empty;

            if (DisplayDirection != "1") 
            {
            script = string.Format("$( document ).ready(function() {1}\n           $( \".mcdiv{0}\" ).css('display', 'inline');\n{2});", QID, "{", "}");
                
            }
            if (IsRequired == "yes")
            {
                rfvMCChk.Enabled = true;
                rfvMCChk.ClientValidationFunction = "rfvMCChkClientScript" + QID;
                script = string.Format("{3}\n\nfunction rfvMCChkClientScript{0}(sender, args) {1}\n  var isValid = false; \n$( \".ch{0} input:first-child\" ).each(function(index, value){1}\n   isValid = isValid || $(this).attr('checked');     \n{2});  \nargs.IsValid = isValid;\n{2}", QID, "{", "}", script);

            }
            if (script.Length > 0)
            {
                var csm = this.Page.ClientScript;
                script = string.Format("<script type='text/javascript'>\n{0}\n</script>", script);
                csm.RegisterClientScriptBlock(GetType(), "jq" + QID, script);
            }

        }


        public void RadioBoxScripts()
        {
            var script= string.Empty;
            var csm = this.Page.ClientScript;

            script = string.Format("$( document ).ready(function() {1}\n           $( \".mcdiv{0}\" ).css('display', 'inline');\n{2});", QID, "{", "}");
            script = string.Format("<script type='text/javascript'>\n{0}\n</script>", script);
            csm.RegisterClientScriptBlock(GetType(), "jq" + QID, script);
        }

        public void Save(int srid)
        {
            SRID = srid.ToString();
            Save();
        }


        public void Save()
        {
            if (SRID == "0" || SRID == "") return;

            

            int qtype = 0;
            int.TryParse(lblQType.Text, out qtype);

            switch (qtype)
            {
                case 3: //freeform
                    SaveFreeForm();
                    break;
                case 2: //multichoice
                    SaveMultiChoice();
                    break;
                case 4: // matrix
                    SaveMatrix();
                    break;
            }
            

        }

        public void SaveFreeForm()
        {
            int SRPK = int.Parse(SRID);
            var obj1 = SurveyResults.FetchObject(SRPK);
            var obj2 = new SurveyAnswers();

            obj2.SRID = obj1.SRID;
            obj2.TenID = obj1.TenID;
            obj2.PID = obj1.PID;
            obj2.SID = obj1.SID;

            obj2.QID = int.Parse(QID);
            obj2.DateAnswered = DateTime.Now;
            obj2.QType = 3;
            obj2.FreeFormAnswer = txtAnswer.Text;
            obj2.Insert();

        }

        public void SaveMultiChoice()
        {
            int SRPK = int.Parse(SRID);
            var obj1 = SurveyResults.FetchObject(SRPK);
            var obj2 = new SurveyAnswers();

            obj2.SRID = obj1.SRID;
            obj2.TenID = obj1.TenID;
            obj2.PID = obj1.PID;
            obj2.SID = obj1.SID;

            obj2.QID = int.Parse(QID);
            obj2.DateAnswered = DateTime.Now;
            obj2.QType = 2;

            if (DisplayControl == "1")
            {
                foreach (RepeaterItem rChkI in rptrChk.Items)
                {
                    var SQCID = rChkI.FindControl("SQCID") as Label;
                    var chkAns = rChkI.FindControl("chkAns") as CheckBox;
                    var chkClar = rChkI.FindControl("txtChkClarification") as TextBox;

                    if (chkAns.Checked)
                    {
                        obj2.ChoiceAnswerIDs = string.Format("{0}{1}{2}",
                                                             (obj2.ChoiceAnswerIDs.Length > 0 ? obj2.ChoiceAnswerIDs : ""),
                                                             (obj2.ChoiceAnswerIDs.Length > 0 ? "," : ""), SQCID.Text);
                        obj2.ClarificationText = string.Format("{0}{1}{2}",
                                                            (obj2.ClarificationText.Length > 0 ? obj2.ClarificationText : ""),
                                                            (obj2.ClarificationText.Length > 0 ? "~|~" : ""), chkClar.Text);
                        obj2.ChoiceAnswerText = string.Format("{0}{1}{2}",
                                                            (obj2.ChoiceAnswerText.Length > 0 ? obj2.ChoiceAnswerText : ""),
                                                            (obj2.ChoiceAnswerText.Length > 0 ? "~|~" : ""), chkAns.Text);
                    }

                }
            }
            if (DisplayControl == "2")
            {
                foreach (RepeaterItem rRdoI in rptrRadio.Items)
                {
                    var SQCID = rRdoI.FindControl("SQCID") as Label;
                    var rbAns = rRdoI.FindControl("rbAns") as RadioButton;
                    var rbClar = rRdoI.FindControl("txtRBClarification") as TextBox;

                    if (rbAns.Checked)
                    {
                        obj2.ChoiceAnswerIDs = string.Format("{0}{1}{2}",
                                                             (obj2.ChoiceAnswerIDs.Length > 0 ? obj2.ChoiceAnswerIDs : ""),
                                                             (obj2.ChoiceAnswerIDs.Length > 0 ? "," : ""), SQCID.Text);
                        obj2.ClarificationText = string.Format("{0}{1}{2}",
                                                            (obj2.ClarificationText.Length > 0 ? obj2.ClarificationText : ""),
                                                            (obj2.ClarificationText.Length > 0 ? "~|~" : ""), rbClar.Text);
                        obj2.ChoiceAnswerText = string.Format("{0}{1}{2}",
                                                            (obj2.ChoiceAnswerText.Length > 0 ? obj2.ChoiceAnswerText : ""),
                                                            (obj2.ChoiceAnswerText.Length > 0 ? "~|~" : ""), rbAns.Text);
                    }

                }
            }
            if (DisplayControl == "3")
            {
                obj2.ChoiceAnswerIDs = ddMultipleChoice.SelectedValue;
                var ch = SQChoices.FetchObject(int.Parse(ddMultipleChoice.SelectedValue));
                var ans = "N/A";
                if (ch != null) ans = ch.ChoiceText;
                obj2.ChoiceAnswerText = ans;
                obj2.ClarificationText = txtDDClarification.Text;
            }

            obj2.Insert();
        }

        public void SaveMatrix()
        {
            int SRPK = int.Parse(SRID);
            var ds = SQChoices.GetAll(int.Parse(QID));

            foreach (RepeaterItem rLine in rptrMRows.Items)
            {
                var obj1 = SurveyResults.FetchObject(SRPK);
                var obj2 = new SurveyAnswers();

                obj2.SRID = obj1.SRID;
                obj2.TenID = obj1.TenID;
                obj2.PID = obj1.PID;
                obj2.SID = obj1.SID;

                obj2.QID = int.Parse(QID);
                obj2.DateAnswered = DateTime.Now;
                obj2.QType = 4;

                var SQMLID = rLine.FindControl("SQMLID") as Label;
                obj2.SQMLID = int.Parse(SQMLID.Text);

                if (DisplayControl == "1")
                {
                    var rptrCheckCols = rLine.FindControl("rptrCheckCols") as Repeater;
                    foreach (RepeaterItem rChkI in rptrCheckCols.Items)
                    {
                        var SQCID = rChkI.FindControl("SQCID") as Label;
                        var chkAns = rChkI.FindControl("rbChoice") as CheckBox;
                        var ChoiceOrder = rChkI.FindControl("ChoiceOrder") as Label;
                        var AnsText = ds.Tables[0].Rows[int.Parse(ChoiceOrder.Text) - 1]["ChoiceText"].ToString();
                        if (chkAns.Checked)
                        {
                            obj2.ChoiceAnswerIDs = string.Format("{0}{1}{2}",
                                                                 (obj2.ChoiceAnswerIDs.Length > 0 ? obj2.ChoiceAnswerIDs : ""),
                                                                 (obj2.ChoiceAnswerIDs.Length > 0 ? "," : ""), SQCID.Text);


                            obj2.ChoiceAnswerText = string.Format("{0}{1}{2}",
                                                                (obj2.ChoiceAnswerText.Length > 0 ? obj2.ChoiceAnswerText : ""),
                                                                (obj2.ChoiceAnswerText.Length > 0 ? "~|~" : ""), AnsText);
                        }

                    }
                }
                if (DisplayControl == "2")
                {
                    var rptrRadioCols = rLine.FindControl("rptrRadioCols") as Repeater;
                    foreach (RepeaterItem rRdoI in rptrRadioCols.Items)
                    {
                        var SQCID = rRdoI.FindControl("SQCID") as Label;
                        var rbAns = rRdoI.FindControl("rbChoice") as RadioButton;
                        var ChoiceOrder = rRdoI.FindControl("ChoiceOrder") as Label;
                        var AnsText = ds.Tables[0].Rows[int.Parse(ChoiceOrder.Text) - 1]["ChoiceText"].ToString();
                        

                        if (rbAns.Checked)
                        {
                            obj2.ChoiceAnswerIDs = string.Format("{0}{1}{2}",
                                                                 (obj2.ChoiceAnswerIDs.Length > 0 ? obj2.ChoiceAnswerIDs : ""),
                                                                 (obj2.ChoiceAnswerIDs.Length > 0 ? "," : ""), SQCID.Text);
                            obj2.ChoiceAnswerText = string.Format("{0}{1}{2}",
                                                                (obj2.ChoiceAnswerText.Length > 0 ? obj2.ChoiceAnswerText : ""),
                                                                (obj2.ChoiceAnswerText.Length > 0 ? "~|~" : ""), AnsText);
                        }

                    }
                }

                obj2.Insert();

            }

        }

        public int JumpLogicActivated()
        {
            int qtype = 0;
            int.TryParse(lblQType.Text, out qtype);

            switch (qtype)
            {
                case 1:
                    return -1;
                case 3:
                    return -1;
                case 2:
                    var ds1 = SQChoices.GetAll(int.Parse(QID));
                    if (DisplayControl == "1")
                    {
                        foreach (RepeaterItem rChkI in rptrChk.Items)
                        {
                            var SQCID = rChkI.FindControl("SQCID") as Label;
                            var chkAns = rChkI.FindControl("chkAns") as CheckBox;
                            if (chkAns.Checked)
                            {
                                foreach( DataRow dr1 in ds1.Tables[0].Rows)
                                {
                                    if (Convert.ToString(dr1["SQCID"]) == SQCID.Text && Convert.ToInt32(dr1["JumpToQuestion"]) > 0)
                                        return Convert.ToInt32(dr1["JumpToQuestion"]);
                                }
                            }

                        }
                    }
                    if (DisplayControl == "2")
                    {
                        foreach (RepeaterItem rRdoI in rptrRadio.Items)
                        {
                            var SQCID = rRdoI.FindControl("SQCID") as Label;
                            var rbAns = rRdoI.FindControl("rbAns") as RadioButton;
                            if (rbAns.Checked)
                            {
                                foreach (DataRow dr1 in ds1.Tables[0].Rows)
                                {
                                    if (Convert.ToString(dr1["SQCID"]) == SQCID.Text && Convert.ToInt32(dr1["JumpToQuestion"]) > 0)
                                        return Convert.ToInt32(dr1["JumpToQuestion"]);
                                }
                            }

                        }
                    }
                    if (DisplayControl == "3")
                    {
                        var SQCID = ddMultipleChoice.SelectedValue;
                        foreach (DataRow dr1 in ds1.Tables[0].Rows)
                        {
                            if (Convert.ToString(dr1["SQCID"]) == SQCID && Convert.ToInt32(dr1["JumpToQuestion"]) > 0)
                                return Convert.ToInt32(dr1["JumpToQuestion"]);
                        }
                    }
                    return -1;
                case 4:
                    var ds2 = SQChoices.GetAll(int.Parse(QID));
                    foreach (RepeaterItem rLine in rptrMRows.Items)
                    {
                        if (DisplayControl == "1")
                        {
                            var rptrCheckCols = rLine.FindControl("rptrCheckCols") as Repeater;
                            foreach (RepeaterItem rChkI in rptrCheckCols.Items)
                            {
                                var chkAns = rChkI.FindControl("rbChoice") as CheckBox;
                                var ChoiceOrder = rChkI.FindControl("ChoiceOrder") as Label;
                                var JumpQ = Convert.ToInt32(ds2.Tables[0].Rows[int.Parse(ChoiceOrder.Text) - 1]["JumpToQuestion"]);
                                if (chkAns.Checked && JumpQ > 0) 
                                    return JumpQ;
                            }
                        }
                        if (DisplayControl == "2")
                        {
                            var rptrRadioCols = rLine.FindControl("rptrRadioCols") as Repeater;
                            foreach (RepeaterItem rRdoI in rptrRadioCols.Items)
                            {
                                var rbAns = rRdoI.FindControl("rbChoice") as RadioButton;
                                var ChoiceOrder = rRdoI.FindControl("ChoiceOrder") as Label;
                                var JumpQ = Convert.ToInt32(ds2.Tables[0].Rows[int.Parse(ChoiceOrder.Text) - 1]["JumpToQuestion"]);
                                if (rbAns.Checked && JumpQ > 0)
                                    return JumpQ;
                            }
                        }
                    }
                    return -1;                    
                case 5:
                    return -1;
                case 6:
                    return -1;
            }
            return -1;
        }
    }
}
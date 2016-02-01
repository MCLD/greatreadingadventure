using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Controls
{
    public partial class EvtCustFldCtl : System.Web.UI.UserControl
    {
        public string Value
        {
            get
            {
                if (DD.Visible) return DD.SelectedValue;
                return Txt.Text;
            }
            set
            {
                InputVal.Text = value;
            }
        }

        public int FieldNumber
        {
            set
            {
                ViewState[this.ID + "_FN"] = value;
            }
            get { 
                var i = FormatHelper.SafeToInt(ViewState[this.ID + "_FN"].ToString());
                if (i == 0) 
                { 
                    i = 1;
                    FieldNumber = i;
                }
                return i;
            }
        }

        public void Render()
        {
            var o = DAL.CustomEventFields.FetchObject();
            if (FieldNumber == 1)
            {
                if (!o.Use1)
                {
                    Lbl.Text = string.Format("Custom Field {0} not enabled", FieldNumber);
                    Txt.Visible = DD.Visible = false;
                    Txt.Text= string.Empty;
                    DD.SelectedValue= string.Empty;
                }
                else
                {
                    Lbl.Text = string.Format("{0}: ", o.Label1);
                    if (o.DDValues1 == "")
                    {
                        Txt.Visible = true;
                        DD.Visible = false;
                        Txt.Text = InputVal.Text;
                        DD.SelectedValue= string.Empty;
                    }
                    else
                    {
                        Txt.Text= string.Empty;
                        Txt.Visible = false;

                        DD.Visible = true;
                        TypeID.Text = o.DDValues1;
                        odsDD.DataBind();
                        
                        DD.Items.Clear();
                        DD.Items.Add(new ListItem("[Select a Value]", ""));
                        DD.DataSourceID = "odsDD"; 
                        DD.DataBind();

                        var i = DD.Items.FindByValue(InputVal.Text);
                        if (i != null) DD.SelectedValue = InputVal.Text;

                    }
                }
            }
            //
            if (FieldNumber == 2)
            {
                if (!o.Use2)
                {
                    Lbl.Text = string.Format("Custom Field {0} not enabled", FieldNumber);
                    Txt.Visible = DD.Visible = false;
                    Txt.Text= string.Empty;
                    DD.SelectedValue= string.Empty;
                }
                else
                {
                    Lbl.Text = string.Format("{0}: ", o.Label2);
                    if (o.DDValues2 == "")
                    {
                        Txt.Visible = true;
                        DD.Visible = false;
                        Txt.Text = InputVal.Text;
                        DD.SelectedValue= string.Empty;
                    }
                    else
                    {
                        Txt.Text= string.Empty;
                        Txt.Visible = false;

                        DD.Visible = true;
                        TypeID.Text = o.DDValues2;
                        odsDD.DataBind();

                        DD.Items.Clear();
                        DD.Items.Add(new ListItem("[Select a Value]", ""));
                        DD.DataSourceID = "odsDD";
                        DD.DataBind();

                        var i = DD.Items.FindByValue(InputVal.Text);
                        if (i != null) DD.SelectedValue = InputVal.Text;
                    }
                }
            }
            //
            if (FieldNumber == 3)
            {
                if (!o.Use3)
                {
                    Lbl.Text = string.Format("Custom Field {0} not enabled", FieldNumber);
                    Txt.Visible = DD.Visible = false;
                    Txt.Text= string.Empty;
                    DD.SelectedValue= string.Empty;
                }
                else
                {
                    Lbl.Text = string.Format("{0}: ", o.Label3);
                    if (o.DDValues3 == "")
                    {
                        Txt.Visible = true;
                        DD.Visible = false;
                        Txt.Text = InputVal.Text;
                        DD.SelectedValue= string.Empty;
                    }
                    else
                    {
                        Txt.Text= string.Empty;
                        Txt.Visible = false;

                        DD.Visible = true;
                        TypeID.Text = o.DDValues3;
                        odsDD.DataBind();

                        DD.Items.Clear();
                        DD.Items.Add(new ListItem("[Select a Value]", ""));
                        DD.DataSourceID = "odsDD";
                        DD.DataBind();

                        var i = DD.Items.FindByValue(InputVal.Text);
                        if (i != null) DD.SelectedValue = InputVal.Text;
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Render();
            }
        }
    }
}

                

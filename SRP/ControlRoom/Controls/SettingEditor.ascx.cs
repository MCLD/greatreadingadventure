using System;
using System.Web.UI.WebControls;

namespace GRA.SRP.ControlRoom.Controls {
    public partial class SettingEditor : System.Web.UI.UserControl {
        public string Type
        {
            get
            {
                if(null != ViewState["_StorageType_"]) {
                    return ViewState["_StorageType_"] as string;
                }
                return "";
            }
            set
            {
                ViewState["_StorageType_"] = value;
            }
        }

        public string EditType
        {
            get
            {
                if(null != ViewState["_EditType_"]) {
                    return ViewState["_EditType_"] as string;
                }
                return "";
            }
            set
            {
                ViewState["_EditType_"] = value;
                switch(value.ToLower()) {
                    case "textbox":
                        uxTextBox.Visible = true;
                        break;
                    case "multilinetextbox":
                        uxMultitext.Visible = true;
                        break;
                    case "html":
                        uxEditor.Visible = true;
                        break;
                    case "checkbox":
                        uxCheckBox.Visible = true;
                        break;
                    case "radio":
                        uxRadio.Visible = true;
                        break;
                    case "dropdown":
                        uxDrop.Visible = true;
                        break;
                    case "date":
                        uxDate.Visible = true;
                        break;
                    default:
                        uxTextBox.Visible = true;
                        break;
                }
            }
        }

        public string ValueList
        {
            get
            {
                if(null != ViewState["_ValueList_"]) {
                    return ViewState["_ValueList_"] as string;
                }
                return "";
            }
            set
            {
                ViewState["_ValueList_"] = value;
            }
        }

        public string Default
        {
            get
            {
                if(null != ViewState["_Default_"]) {
                    return ViewState["_Default_"] as string;
                }
                return "";
            }
            set
            {
                ViewState["_Default_"] = value;
            }
        }

        public string SID
        {
            get
            {
                if(null != ViewState["_SID_"]) {
                    return ViewState["_SID_"] as string;
                }
                return "";
            }
            set
            {
                ViewState["_SID_"] = value;
            }
        }

        public string Value
        {
            get
            {
                switch(EditType.ToLower()) {
                    case "textbox":
                        return uxTextBox.Text;
                    case "multilinetextbox":
                        return uxMultitext.Text;
                    case "html":
                        return uxEditor.InnerHtml;
                    case "checkbox":
                        return uxCheckBox.Checked.ToString();
                    case "radio":
                        return uxRadio.SelectedValue;
                    case "dropdown":
                        return uxDrop.SelectedValue;
                    case "date":
                        return uxDate.Text;

                    default:
                        return uxTextBox.Text;
                }
            }
            set
            {
                switch(EditType.ToLower()) {
                    case "textbox":
                        uxTextBox.Text = value;
                        break;
                    case "multilinetextbox":
                        uxMultitext.Text = value;
                        break;
                    case "html":
                        uxEditor.InnerHtml = value;
                        break;
                    case "checkbox":
                        try {
                            uxCheckBox.Checked = (bool.Parse(value));
                        } catch {
                        }
                        break;
                    case "radio":
                        string[] items = ValueList.Split(';');
                        foreach(var item in items) {
                            string[] oneItem = item.Split(',');
                            uxRadio.Items.Add(new ListItem(oneItem[1], oneItem[0]));
                            if(value == oneItem[0])
                                uxRadio.SelectedValue = value;
                        }

                        uxRadio.Visible = true;
                        break;
                    case "dropdown":
                        string[] ddItems = ValueList.Split(';');
                        foreach(var dItem in ddItems) {
                            string[] oneDItem = dItem.Split(',');
                            uxDrop.Items.Add(new ListItem(oneDItem[1], oneDItem[0]));
                            if(value == oneDItem[0])
                                uxDrop.SelectedValue = value;
                        }
                        uxDrop.Visible = true;
                        break;
                    case "date":
                        try {
                            uxDate.Text = DateTime.Parse(value).ToString("MM/dd/yyyy");
                        } catch(Exception) {
                            uxDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
                        }
                        //uxDate.VisibleDate = uxDate.SelectedDate;
                        uxDate.Visible = true;
                        break;
                    default:
                        uxTextBox.Text = value;
                        break;
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e) {

        }
    }
}
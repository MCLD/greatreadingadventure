using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace GRA.Tools {
    public class RegistrationHelper {
        public void BindCustomDDL(RepeaterItemEventArgs e,
                                  DataSet dropdownDataSet,
                                  string ddlControlId,
                                  string tbControlId) {
            var ddlControl = (DropDownList)e.Item.FindControl(ddlControlId);
            ddlControl.Items.Clear();
            ddlControl.Items.Add(new ListItem("[Select a Value]", string.Empty));
            foreach(DataRow row in dropdownDataSet.Tables[0].Rows) {
                var listItem = new ListItem(row["Description"].ToString(),
                                            row["Code"].ToString());
                ddlControl.Items.Add(listItem);
            }

            string tbValue = ((TextBox)e.Item.FindControl(tbControlId)).Text;
            var selected = ddlControl.Items.FindByValue(tbValue);
            if(selected != null) {
                ddlControl.SelectedValue = tbValue;
            }
        }
    }
}

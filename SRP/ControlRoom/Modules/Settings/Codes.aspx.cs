using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Modules.Settings
{
    public partial class Codes : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4100;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Codes Management");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SettingsRibbon());

                if (Request["CTID"] != null && Request["CTID"] != "")
                {
                    LoadData();
                    btnEdit.Visible = true;
                    var i = ddlCodeTypes.Items.FindByValue(Request["CTID"].ToString());
                    if (i != null) 
                    {
                        ddlCodeTypes.SelectedValue = Request["CTID"].ToString();
                        ShowDD();
                    }
                }
            }
        }

        protected void ddlCodeTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDD();
        }

        protected void btnAdd1_Click(object sender, EventArgs e)
        {
            btnEdit.Visible = false;
            btnSelect.Visible = false;
            btnAdd1.Visible = false;
            btnCTDelete.Visible = false;

            pnlEdit.Visible = true;
            pnlCodes.Visible = false;
            pnlSelect.Visible = false;

            CTID.Text = CodeTypeValue.Text = CodeTypeDesc.Text= string.Empty;
            CodeTypeValue.ReadOnly = false;
            CodeTypeValue.Enabled = true;
            lblAE.Text = "Add";
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            btnEdit.Visible = false;
            btnSelect.Visible = false;
            btnAdd1.Visible = false;

            pnlEdit.Visible = true;
            pnlCodes.Visible = false;
            pnlSelect.Visible = false;

            lblAE.Text = "Edit";
            var ct = new DAL.CodeType();
            ct.Fetch(int.Parse(ddlCodeTypes.SelectedValue.ToString()));
            CodeTypeValue.Text = ct.CodeTypeName;
            CodeTypeDesc.Text = ct.Description;
            CTID.Text = ct.CTID.ToString();
            if (ct.isSystem)
            {
                CodeTypeValue.ReadOnly = true;
                CodeTypeValue.Enabled = false;

                btnCTDelete.Visible = false;
            }
            else
            {
                CodeTypeValue.ReadOnly = false;
                CodeTypeValue.Enabled = true;

                btnCTDelete.Visible = true; 
            }
        }

        protected void btnCTDelete_Click(object sender, ImageClickEventArgs e)
        {
            var key = int.Parse(CTID.Text);
            try
            {
                var obj = new CodeType();
                if (obj.IsValid(BusinessRulesValidationMode.DELETE))
                {
                    CodeType.FetchObject(key).Delete();
                    ddlCodeTypes.SelectedValue = "0";
                    LoadData();
                    ddlCodeTypes_SelectedIndexChanged(null, null);

                    var masterPage = (IControlRoomMaster)Master;
                    if (masterPage != null) masterPage.PageMessage = SRPResources.DeleteOK;
                }
                else
                {
                    var masterPage = (IControlRoomMaster)Master;
                    string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                    foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                    {
                        message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                    }
                    message = string.Format("{0}</ul>", message);
                    if (masterPage != null) masterPage.PageError = message;
                }
            }
            catch (Exception ex)
            {
                var masterPage = (IControlRoomMaster)Master;
                if (masterPage != null)
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
            }

        }

        public void LoadData()
        {
            odsCT.DataBind();
            while (ddlCodeTypes.Items.Count > 1)
            {
                ddlCodeTypes.Items.RemoveAt(1);
            }            
            ddlCodeTypes.DataBind();
        }

        protected void btnBack_Click(object sender, ImageClickEventArgs e)
        {
            ShowDD();
        }

        public void ShowDD()
        {
            if (ddlCodeTypes.SelectedValue == "0")
            {
                btnEdit.Visible = false;
                btnSelect.Visible = true;
                btnAdd1.Visible = true;

                // show/hide other panels
                pnlEdit.Visible = false;
                pnlCodes.Visible = false;
                pnlSelect.Visible = true;
                //ClearCodesPanel();
            }
            else
            {
                btnEdit.Visible = true;
                btnSelect.Visible = false;
                btnAdd1.Visible = true;

                // show/hide other panels
                pnlEdit.Visible = false;
                pnlCodes.Visible = true;
                pnlSelect.Visible = true;
                //LoadCodesPanel();
            }

            CodeAdd.Text = DescriptionAdd.Text= string.Empty;

            var ds = DAL.Codes.GetAlByTypeID(int.Parse(ddlCodeTypes.SelectedValue));
            rptrCodes.DataSource = ds;
            rptrCodes.DataBind();

        }

        protected void btnCTSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                var obj = new CodeType();
                var isEdit = false;
                int pk = 0;
                if (CTID.Text != "")
                {
                    pk = int.Parse(CTID.Text);
                    obj.Fetch(pk);
                    isEdit = true;
                }

                if (!isEdit) obj.isSystem = false;
                obj.Description = CodeTypeDesc.Text;
                obj.CodeTypeName = CodeTypeValue.Text;

                if (isEdit)
                {
                    if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        obj.Update();
                        LoadData();
                        ShowDD();

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = SRPResources.SaveOK;
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                        {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        masterPage.PageError = message;
                    }
                }
                else
                {
                    if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                    {
                        pk = obj.Insert();
                        LoadData();
                        ddlCodeTypes.SelectedValue = pk.ToString();
                        ShowDD();

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = SRPResources.AddedOK;
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                        {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        masterPage.PageError = message;
                    }                    
                }
                
            }
            catch (Exception ex)
            {
                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
            }
        }

        protected void btnAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                var obj = new DAL.Codes();
                obj.CTID = int.Parse(ddlCodeTypes.SelectedValue.ToString());
                obj.Code = CodeAdd.Text;
                obj.Description = DescriptionAdd.Text;

              
                if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                {
                    obj.Insert();                    
                    LoadData();
                    ddlCodeTypes.SelectedValue = obj.CTID.ToString();
                    ShowDD();

                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageMessage = SRPResources.AddedOK;
                }
                else
                {
                    var masterPage = (IControlRoomMaster)Master;
                    string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                    foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                    {
                        message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                    }
                    message = string.Format("{0}</ul>", message);
                    masterPage.PageError = message;
                }
                

            }
            catch (Exception ex)
            {
                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
            }
        }

        protected void rptrCodes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRecord")
            {

                var key = int.Parse(e.CommandArgument.ToString());
                try
                {
                    var obj = new DAL.Codes();
                    if (obj.IsValid(BusinessRulesValidationMode.DELETE))
                    {
                        DAL.Codes.FetchObject(key).Delete();
                        ShowDD();

                        var masterPage = (IControlRoomMaster)Master;
                        if (masterPage != null) masterPage.PageMessage = SRPResources.DeleteOK;
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                        {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        if (masterPage != null) masterPage.PageError = message;
                    }
                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    if (masterPage != null)
                        masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }


            }
        }

        protected void btnCodeSave_Click(object sender, ImageClickEventArgs e)
        {
            var rptr = rptrCodes;
            int i = 0;
            bool errors = false;
            foreach (RepeaterItem item in rptr.Items)
            {

                i++;
                try
                {
                    var CID = int.Parse(((TextBox)item.FindControl("CID")).Text);
                    var CTID = int.Parse(((TextBox)item.FindControl("CTID")).Text);
                    var Code = ((TextBox)item.FindControl("Code")).Text;
                    var Description = ((TextBox)item.FindControl("Description")).Text;

                    var obj = new DAL.Codes();
                    obj.Fetch(CID);
                    obj.Code = Code;
                    obj.Description = Description;

                    if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        obj.Update();
                        

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = SRPResources.SaveAllOK;
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format("On Row {1}: " + SRPResources.ApplicationError1, "<ul>", i);
                        foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                        {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        masterPage.PageError = message;
                        errors = true;
                    }


                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format("On Row {1}: " + SRPResources.ApplicationError1, ex.Message, i);
                    errors = true;
                }

            }

            if (!errors) ShowDD();
        }
        
    }
}
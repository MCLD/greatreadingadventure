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
    public partial class LibraryDistrict : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4100;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Library and District Crosswalk");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SettingsRibbon());

                LoadData();
                
            }
        }

        protected  void  LoadData()
        {
            var ds = LibraryCrosswalk.GetAll();
            rptrCW.DataSource = ds;
            rptrCW.DataBind();
        }

        protected void SaveData()
        {
            var rptr = rptrCW;
            int i = 0;
            bool errors = false;
            foreach (RepeaterItem item in rptr.Items)
            {

                i++;
                try
                {
                    var ID = int.Parse(((Label)item.FindControl("ID")).Text);
                    var BranchID = int.Parse(((DropDownList)item.FindControl("BranchID")).SelectedValue);
                    var DistrictID = int.Parse(((DropDownList)item.FindControl("DistrictID")).SelectedValue);
                    var City = ((TextBox)item.FindControl("City")).Text;

                    var obj = new LibraryCrosswalk();
                    if (ID != 0) obj.Fetch(ID);
                    obj.BranchID = BranchID;
                    obj.DistrictID = DistrictID;
                    obj.City = City;

                    if (ID != 0)
                    {
                        obj.Update();
                    }
                    else
                    {
                        obj.Insert();
                    }
                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format("On Row {1}: " + SRPResources.ApplicationError1, ex.Message, i);
                    errors = true;
                }

            }

            if (!errors)
            {
                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageMessage = SRPResources.SaveAllOK;
            }
        }

        protected void btnBack_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/ControlRoom/Modules/Settings/Default.aspx");
        }

        protected void btnRefresh_Click(object sender, ImageClickEventArgs e)
        {
            LoadData();
            var masterPage = (IControlRoomMaster)Master;
            masterPage.PageMessage = SRPResources.RefreshAllOK;
        }

        protected void btnSave_Click(object sender, ImageClickEventArgs e)
        {
            SaveData();
            LoadData();
        }

        protected void btnSaveback_Click(object sender, ImageClickEventArgs e)
        {
            SaveData();
            Response.Redirect("~/ControlRoom/Modules/Settings/Default.aspx");
        }

        protected void rptrCW_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var ctl = (DropDownList)e.Item.FindControl("BranchID");
            var txt = (TextBox)e.Item.FindControl("BranchIDTxt");
            var i = ctl.Items.FindByValue(txt.Text);
            if (i != null) ctl.SelectedValue = txt.Text;

            ctl = (DropDownList)e.Item.FindControl("DistrictID");
            txt = (TextBox)e.Item.FindControl("DistrictIDTxt");
            i = ctl.Items.FindByValue(txt.Text);
            if (i != null) ctl.SelectedValue = txt.Text;

        }
    }
}
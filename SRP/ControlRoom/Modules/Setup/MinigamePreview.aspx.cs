using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class MinigamePreview : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4300;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Minigame Preview");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

            }


            TranslateStrings(this);
        }




#region String Translation

        public void TranslateStrings(System.Web.UI.Control ctl)
        {
            //var i = DAL.Programs.GetDefaultProgramID();
            //StringResources.LoadProgramResourceFile(i.ToString());
            StringResources.LoadProgramResourceFile("default");
            //MasterPage = (BaseSRPMaster)Master;
            //MasterPage.InitResFile();

            LoadLabels(ctl);
            //LoadValidators(ctl);
            //LoadRadioButtonLists(ctl);
            //LoadDropDownListLists(ctl);
            LoadButtons(ctl);
        }

        public string GetResourceString(string resName)
        {

            try
            {
                return StringResources.getString(resName);
            }
            catch //(Exception ex)
            {
                return resName;
            }
        }

        public void TranslateDropDownList(System.Web.UI.Control c)
        {
            if (c is DropDownList)
            {
                var dd = (DropDownList)c;
                foreach (ListItem i in dd.Items)
                {
                    if (((ListItem)i).Value == ((ListItem)i).Text) ((ListItem)i).Value = ((ListItem)i).Text;
                    ((ListItem)i).Text = GetResourceString(((ListItem)i).Text);
                }
            }
        }
        public void LoadDropDownListLists(System.Web.UI.Control ctl)
        {
            foreach (System.Web.UI.Control c in ctl.Controls)
            {
                LoadDropDownListLists(c);
                if (c is DropDownList)
                {
                    var dd = (DropDownList)c;
                    foreach (ListItem i in dd.Items)
                    {
                        if (((ListItem)i).Value == ((ListItem)i).Text) ((ListItem)i).Value = ((ListItem)i).Text;
                        ((ListItem)i).Text = GetResourceString(((ListItem)i).Text);
                    }
                }
            }
        }
        public void TranslateRadioButtonList(System.Web.UI.Control c)
        {
            if (c is RadioButtonList)
            {
                var rb = (RadioButtonList)c;
                foreach (ListItem i in rb.Items)
                {
                    if (((ListItem)i).Value == ((ListItem)i).Text) ((ListItem)i).Value = ((ListItem)i).Text;
                    ((ListItem)i).Text = GetResourceString(((ListItem)i).Text);
                }
            }
        }
        public void TranslateDdList(System.Web.UI.Control c)
        {
            if (c is DropDownList)
            {
                var rb = (DropDownList)c;
                foreach (ListItem i in rb.Items)
                {
                    if (((ListItem)i).Value == ((ListItem)i).Text) ((ListItem)i).Value = ((ListItem)i).Text;
                    ((ListItem)i).Text = GetResourceString(((ListItem)i).Text);
                }
            }
        }
        public void LoadRadioButtonLists(System.Web.UI.Control ctl)
        {
            foreach (System.Web.UI.Control c in ctl.Controls)
            {
                LoadRadioButtonLists(c);
                if (c is RadioButtonList)
                {
                    var rb = (RadioButtonList)c;
                    foreach (ListItem i in rb.Items)
                    {
                        if (((ListItem)i).Value == ((ListItem)i).Text) ((ListItem)i).Value = ((ListItem)i).Text;
                        ((ListItem)i).Text = GetResourceString(((ListItem)i).Text);
                    }
                }
            }
        }
        public void LoadButtons(System.Web.UI.Control ctl)
        {
            foreach (System.Web.UI.Control c in ctl.Controls)
            {
                LoadButtons(c);
                if (c is Button)
                {
                    ((Button)c).Text = GetResourceString(((Button)c).Text);
                }
                if (c is LinkButton)
                {
                    ((LinkButton)c).Text = GetResourceString(((LinkButton)c).Text);
                }
            }
        }
        public void LoadLabels(System.Web.UI.Control ctl)
        {
            foreach (System.Web.UI.Control c in ctl.Controls)
            {
                LoadLabels(c);
                if (c is Label)
                {
                    ((Label)c).Text = GetResourceString(((Label)c).Text);
                }
            }
        }
        public void LoadValidators(System.Web.UI.Control ctl)
        {
            foreach (System.Web.UI.Control c in ctl.Controls)
            {
                LoadValidators(c);
                if (c is RequiredFieldValidator)
                {
                    ((RequiredFieldValidator)c).ErrorMessage = GetResourceString(((RequiredFieldValidator)c).ErrorMessage);
                }
                if (c is RangeValidator)
                {
                    ((RangeValidator)c).ErrorMessage = GetResourceString(((RangeValidator)c).ErrorMessage);
                }
                if (c is RegularExpressionValidator)
                {
                    ((RegularExpressionValidator)c).ErrorMessage = GetResourceString(((RegularExpressionValidator)c).ErrorMessage);
                }
                if (c is CompareValidator)
                {
                    ((CompareValidator)c).ErrorMessage = GetResourceString(((CompareValidator)c).ErrorMessage);
                }
                if (c is CustomValidator)
                {
                    ((CustomValidator)c).ErrorMessage = GetResourceString(((CustomValidator)c).ErrorMessage);
                }

            }
        }

#endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using STG.SRP.ControlRooms;
using STG.SRP.Core.Utilities;
using STG.SRP.DAL;
using STG.SRP.Utilities.CoreClasses;

namespace STG.SRP.ControlRoom.Modules.Setup
{
    public class KeyItem
    {
        public int CBID { get; set; }
        public int Character_Num { get; set; }
        public string Character { get; set; }
    }

    public partial class MGCodeBreakerKeySetup : BaseControlRoomPage
    {



        public List<KeyItem> GetKeyCharacters()
        {
            var CBID = int.Parse(lblCBID.Text);
            List<KeyItem> lst = new List<KeyItem>();

            int currItem = 0;
            for (var i = 65; i <= 90 ; i++)
            {
                var item = new KeyItem { Character_Num = i, Character = Convert.ToChar(i).ToString(), CBID = CBID };
                lst.Add(item);
                currItem = currItem + 1;
            }
            for (var i = 97; i <= 122; i++)
            {
                var item = new KeyItem { Character_Num = i, Character = Convert.ToChar(i).ToString(), CBID = CBID };
                //lst[currItem] = item;
                lst.Add(item);
                currItem = currItem + 1;
            }
            
            //33 46 44 63 (, . ! ? )
            var j = 33;
            var newItem = new KeyItem { Character_Num = j, Character = Convert.ToChar(j).ToString(), CBID = CBID };
            //lst[currItem] = newItem;
            lst.Add(newItem);

            j = 46;
            newItem = new KeyItem { Character_Num = j, Character = Convert.ToChar(j).ToString(), CBID = CBID };
            //lst[currItem] = newItem;
            lst.Add(newItem);

            j = 44;
            newItem = new KeyItem { Character_Num = j, Character = Convert.ToChar(j).ToString(), CBID = CBID };
            //lst[currItem] = newItem;
            lst.Add(newItem);

            j = 63;
            newItem = new KeyItem { Character_Num = j, Character = Convert.ToChar(j).ToString(), CBID = CBID };
            //lst[currItem] = newItem;
            lst.Add(newItem); 
            
            return lst;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
            }

            //MasterPage.RequiredPermission = PERMISSIONID;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Code Breaker Key Setup");

            if (!IsPostBack)
            {
                if (Request["MGID"] != null)
                {
                    lblMGID.Text = Request["MGID"];
                    lblCBID.Text = Request["CBID"];

                    var o = Minigame.FetchObject(int.Parse(lblMGID.Text));
                    AdminName.Text = o.AdminName;
                }
                else
                {
                    Response.Redirect("~/ControlRoom/");
                }
                rptr.DataSource = GetKeyCharacters();
                rptr.DataBind();
            }
        }

        protected void btnBack_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Setup/MGCodeBreakerAddEdit.aspx?PK=" + lblMGID.Text;
            Response.Redirect(returnURL);
        }

        protected void btnRefresh_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }
    }
}
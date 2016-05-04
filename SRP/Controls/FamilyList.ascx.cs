using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;
using GRA.Tools;
using System.IO;

namespace GRA.SRP.Controls
{
    public partial class FamilyList : System.Web.UI.UserControl
    {
        private const string NoAvatarPathSm = "~/images/AvatarCache/no_avatar_sm.png";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ProgramCodeLabel.Visible = false;

                var patron = (Patron)Session["Patron"];
                if (Session[SessionKey.IsMasterAccount] as bool? != true)
                {
                    Response.Redirect("~");
                }

                // populate screen
                var ds = Patron.GetSubAccountList((int)Session["MasterAcctPID"]);
                rptr.DataSource = ds;
                rptr.DataBind();

                ((BaseSRPPage)Page).TranslateStrings(rptr);
            }
        }

        protected void rptr_ItemDataBound(object source, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item
               || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var patronRecord = (DataRowView)e.Item.DataItem;
                var patronId = patronRecord["PID"].ToString();
                var avatarState = patronRecord["AvatarState"].ToString();

                string potentialAvatarPath = null;
                string avatarPathSm = NoAvatarPathSm;
                string avatarPathMd = null;
                string avatarPathLg = null;

                potentialAvatarPath = string.Format("~/images/AvatarCache/sm_{0}.png",
                                                    avatarState);
                if (File.Exists(Server.MapPath(potentialAvatarPath)))
                {
                    avatarPathSm = potentialAvatarPath;
                }

                potentialAvatarPath = string.Format("~/images/AvatarCache/md_{0}.png",
                                                    avatarState);
                if (File.Exists(Server.MapPath(potentialAvatarPath)))
                {
                    avatarPathMd = potentialAvatarPath;
                }

                potentialAvatarPath = string.Format("~/images/AvatarCache/{0}.png",
                                    avatarState);
                if (File.Exists(Server.MapPath(potentialAvatarPath)))
                {
                    avatarPathLg = potentialAvatarPath;
                }

                var image = (Image)e.Item.FindControl("avatarImage");
                image.ImageUrl = avatarPathSm;
                if (!string.IsNullOrEmpty(avatarPathMd))
                {
                    string srcSet = string.Format("{0} 1x, {1} 2x",
                                                  VirtualPathUtility.ToAbsolute(avatarPathSm),
                                                  VirtualPathUtility.ToAbsolute(avatarPathMd));
                    image.Attributes.Add("srcset", srcSet);
                }

                var avatarLink = (HyperLink)e.Item.FindControl("avatarLink");
                if (!string.IsNullOrEmpty(avatarPathLg))
                {
                    avatarLink.NavigateUrl = avatarPathLg;
                }

                var scoreControl = e.Item.FindControl("CurrentScore") as Label;
                if(scoreControl != null)
                {
                    var points = PatronPoints.GetTotalPatronPoints((int)patronRecord["PID"]);
                    scoreControl.Text = string.Format("{0} points", points);
                }

                var rewardLabel = e.Item.FindControl("ProgramRewardCodes") as Label;
                var rewardCodesData = DAL.ProgramCodes.GetAllForPatron((int)patronRecord["PID"]);
                if (rewardCodesData != null
                    && rewardCodesData.Tables.Count > 0
                    && rewardCodesData.Tables[0].Rows.Count > 0)
                {
                    var codes = rewardCodesData.Tables[0]
                        .AsEnumerable()
                        .Select(r => r.Field<string>("ShortCode"))
                        .ToArray();
                    rewardLabel.Text = string.Join("<br>", codes);
                    ProgramCodeLabel.Visible = true;
                }
                else
                {
                    rewardLabel.Text = string.Empty;
                }

            }
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Session["SA"] = "0";
            if (e.CommandName == "pwd")
            {
                Session["SA"] = e.CommandArgument.ToString();
                Response.Redirect("~/Account/ChangeFamMemberPwd.aspx");

            }
            if (e.CommandName == "log")
            {
                Session["SA"] = e.CommandArgument.ToString();
                Response.Redirect("~/Account/EnterFamMemberLog.aspx");

            }
            if (e.CommandName == "login")
            {
                var newPID = int.Parse(e.CommandArgument.ToString());

                if ((int)Session["MasterAcctPID"] != newPID
                   && !Patron.CanManageSubAccount((int)Session["MasterAcctPID"], newPID))
                {
                    // kick them out
                    Response.Redirect("~");
                }

                var newPatron = Patron.FetchObject(newPID);
                new SessionTools(Session).EstablishPatron(newPatron);
                //Session["Patron"] = bp;
                //Session["ProgramID"] = bp.ProgID;
                //Session["TenantID"] = bp.TenID;

                TestingBL.CheckPatronNeedsPreTest();
                TestingBL.CheckPatronNeedsPreTest();

                Response.Redirect("~");
            }
        }
    }
}
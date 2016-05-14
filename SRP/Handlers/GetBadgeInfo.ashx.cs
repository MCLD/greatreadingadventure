using GRA.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace GRA.SRP.Handlers
{
    public class JsonBadge : JsonBase
    {
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string[] Earn { get; set; }
        public string DateEarned { get; set; }
        public bool Hidden { get; set; }
    }

    /// <summary>
    /// Return a badge's information via JSON
    /// </summary>
    public class GetBadgeInfo : IHttpHandler, IRequiresSessionState
    {
        private const string NoBadgePath = "~/images/Badges/no_badge.png";

        public void ProcessRequest(HttpContext context)
        {
            JsonBadge jsonResponse = null;
            try
            {
                if (context.Request.QueryString["BadgeId"] == null)
                {
                    throw new Exception("No badge id provided.");
                }

                int badgeId = 0;
                if (!int.TryParse(context.Request["BadgeId"].ToString(), out badgeId))
                {
                    throw new Exception("Invalid badge id provided.");
                }

                int patronId = 0;
                var patron = context.Session[SessionKey.Patron] as DAL.Patron;
                if (patron != null)
                {
                    patronId = patron.PID;
                }


                var badgeDetails = new Logic.Badge().GetForDisplay(context.Server, 
                    badgeId,
                    patronId);

                if (badgeDetails.Hidden == true && badgeDetails.Earned == false)
                {
                    throw new Exception("Secret badge must be earned to be revealed.");
                }

                jsonResponse = new JsonBadge()
                {
                    UserName = badgeDetails.DisplayName,
                    ImageUrl = VirtualPathUtility.ToAbsolute(badgeDetails.ImageUrl),
                    Earn = badgeDetails.HowToEarn,
                    Description = context.Server.HtmlDecode(badgeDetails.Description),
                    DateEarned = badgeDetails.DateEarned,
                    Hidden = badgeDetails.Hidden,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                string safeBadgeId = context.Request["BadgeId"] == null
                    ? "<none requested>"
                    : context.Request["BadgeId"].ToString();
                this.Log().Error("Requested badge {0} from {1}?{2} - {3}",
                    safeBadgeId,
                    context.Request.Url,
                    context.Request.QueryString,
                    ex.Message);
                jsonResponse = new JsonBadge()
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }

            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(jsonResponse));
        }

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}

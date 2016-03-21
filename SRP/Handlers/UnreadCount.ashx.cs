using GRA.SRP.DAL;
using GRA.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace GRA.SRP.Handlers
{
    public class JsonUnread : JsonBase
    {
        public int UnreadMessages { get; set; }
    }

    /// <summary>
    /// Summary description for UnreadCount
    /// </summary>
    public class UnreadCount : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            var jsonResponse = new JsonUnread
            {
                Success = false
            };
            try
            {
                var patronSession = context.Session[SessionKey.Patron] as Patron;
                if (patronSession != null)
                {
                    var unreadLookup = Notifications.GetAllUnreadToPatron(patronSession.PID);
                    if (unreadLookup != null && unreadLookup.Tables.Count > 0)
                    {
                        jsonResponse.UnreadMessages = unreadLookup.Tables[0].Rows.Count;
                        jsonResponse.Success = true;
                    } else
                    {
                        jsonResponse.ErrorMessage = "Could not fetch list of messages.";
                    }
                }
                else
                {
                    jsonResponse.ErrorMessage = "Could not find patron session.";
                }

            }
            catch (Exception ex)
            {
                this.Log().Error("Unread lookup error: {0} - {1}",
                    ex.Message,
                    ex.StackTrace);
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
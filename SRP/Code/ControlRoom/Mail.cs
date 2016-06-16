using GRA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GRA.SRP.Code.ControlRoom
{
    public class Mail
    {
        public void ClearUnreadCrMailCache(HttpContext context)
        {
            var tenant = context.Session[GRA.SRP.ControlRoom.CRSessionKey.TenantID];
            int tenantId = tenant as int? ?? -1;
            if (tenantId == -1)
            {
                return;
            }

            new SessionTools(context.Session).RemoveCache(context.Cache,
                GRA.SRP.ControlRoom.CRSessionKey.UnreadMail,
                tenantId);
        }

        public int UnreadCrMail(HttpContext context)
        {
            if (context.Session == null)
            {
                return 0;
            }
            var tenant = context.Session[GRA.SRP.ControlRoom.CRSessionKey.TenantID];
            int tenantId = tenant as int? ?? -1;
            if (tenantId == -1)
            {
                return 0;
            }

            var sessionTool = new SessionTools(context.Session);
            var cachedFeed = sessionTool.GetCache(context.Cache,
                GRA.SRP.ControlRoom.CRSessionKey.UnreadMail,
                tenantId) as int?;
            if (cachedFeed != null)
            {
                return (int)cachedFeed;
            }
            else
            {
                var unread = SRP.DAL.Notifications.GetAllUnreadToPatron(0);
                if (unread != null && unread.Tables.Count > 0)
                {
                    int unreadMessages = unread.Tables[0].Rows.Count;
                    sessionTool.SetCache(context.Cache,
                        GRA.SRP.ControlRoom.CRSessionKey.UnreadMail,
                        unreadMessages,
                        tenantId,
                        60);
                    return unreadMessages;
                }
                return 0;
            }
        }
    }
}
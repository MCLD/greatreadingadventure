using GRA.Tools;
using Newtonsoft.Json;
using SRP_DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace GRA.SRP.Handlers {
    public class JsonFeedEntry {
        public int ID { get; set; }
        public int AvatarId { get; set; }
        public string Username { get; set; }
        public string AwardedAt { get; set; }
        public int AwardReasonId { get; set; }
        public int BadgeId { get; set; }
        public int ChallengeId { get; set; }
        public string AchievementName { get; set; }
    }
    public class JsonFeed : JsonBase {
        public JsonFeedEntry[] Entries { get; set; }
        public int Latest { get; set; }
    }

    public class Feed : IHttpHandler, IRequiresSessionState {
        public void ProcessRequest(HttpContext context) {
            var tenant = context.Session["TenantID"];
            int tenantId = tenant as int? ?? -1;
            if(tenantId == -1) {
                tenantId = Core.Utilities.Tenant.GetMasterID();
            }

            string cacheKey = string.Format("{0}.{1}", CacheKey.Feed, tenantId);

            try {
                if(context.Cache[cacheKey] != null) {
                    var cachedJsonResponse = context.Cache[cacheKey] as JsonFeed;
                    if(cachedJsonResponse != null) {
                        context.Response.ContentType = "application/json";
                        context.Response.Write(JsonConvert.SerializeObject(cachedJsonResponse));
                        return;
                    }
                }
            } catch (Exception ex) {
                this.Log().Error("Error looking up feed data in cache: {0}", ex.Message);
            }

            var jsonResponse = new JsonFeed();
            var entries = new List<JsonFeedEntry>();

            int after = 0;
            int.TryParse(context.Request.QueryString["after"], out after);

            //p.[AvatarID], p.[Username], bl.ListName, b.[UserName] as BadgeName, pp.[PPID], pp.[AwardDate], pp.[AwardReasonCd], pp.[BadgeId]
            try {
                var feed = new ActivityFeed().Latest(after, tenantId);
                foreach(DataRow dataRow in feed.Rows) {
                    var entry = new JsonFeedEntry {
                        ID = (int)dataRow["PPID"],
                        AvatarId = (int)dataRow["AvatarID"],
                        Username = (string)dataRow["Username"],
                        AwardedAt = ((DateTime)dataRow["AwardDate"]).ToString(),
                        AwardReasonId = (int)dataRow["AwardReasonCd"],
                        BadgeId = (int)dataRow["BadgeId"],
                        ChallengeId = dataRow["BLID"] == DBNull.Value ? 0 : (int)dataRow["BLID"]
                    };

                    if(entry.ID > jsonResponse.Latest) {
                        jsonResponse.Latest = entry.ID;
                    }

                    switch(entry.AwardReasonId) {
                        case 1:
                            // got badge
                            entry.AchievementName = (string)dataRow["BadgeName"];
                            break;
                        case 2:
                            // completed challenge
                            entry.AchievementName = (string)dataRow["ListName"];
                            break;
                        case 4:
                            entry.AchievementName = (string)dataRow["GameName"];
                            break;
                    }
                    entries.Add(entry);
                }

                jsonResponse.Entries = entries.ToArray();
                jsonResponse.Success = true;
            } catch (Exception ex) {
                this.Log().Error("Error loading feed: {0}", ex.Message);
                jsonResponse.Success = false;
            }

            if(jsonResponse.Success) {
                try {
                    DateTime cacheUntil = DateTime.UtcNow.AddSeconds(30);
                    if(context.Cache[cacheKey] == null) {
                        //this.Log().Debug("Caching feed data until {0}",
                        //                 cacheUntil.ToLocalTime().ToLongTimeString());
                        context.Cache.Insert(cacheKey,
                                             jsonResponse,
                                             null,
                                             cacheUntil,
                                             System.Web.Caching.Cache.NoSlidingExpiration,
                                             System.Web.Caching.CacheItemPriority.Default,
                                             null);
                    }
                } catch (Exception ex) {
                    this.Log().Error("Error caching feed response: {0}", ex.Message);
                }
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
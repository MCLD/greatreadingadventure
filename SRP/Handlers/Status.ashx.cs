using GRA.Tools;
using Newtonsoft.Json;
using SRP_DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace GRA.SRP.Handlers
{
    public class JsonStatus : JsonBase
    {
        public int PointsEarned { get; set; }
        public int BadgesAwarded { get; set; }
        public int ChallengesCompleted { get; set; }
        public string Since { get; set; }
    }

    public class Status : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            var tenant = context.Session["TenantID"];
            int tenantId = tenant as int? ?? -1;
            if (tenantId == -1)
            {
                tenantId = Core.Utilities.Tenant.GetMasterID();
            }

            var sessionTool = new SessionTools(context.Session);
            var cachedStatus = sessionTool.GetCache(context.Cache, CacheKey.Feed, tenantId) as JsonStatus;

            try
            {
                if (cachedStatus != null)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.Write(JsonConvert.SerializeObject(cachedStatus));
                    return;
                }
            }
            catch (Exception ex)
            {
                this.Log().Error("Error looking up status data in cache: {0}", ex.Message);
            }

            var jsonResponse = new JsonStatus();
            try
            {
                TenantStatusReport result = null;
                result = new TenantStatus(tenantId).CurrentStatus();

                jsonResponse.PointsEarned = result.PointsEarned;
                jsonResponse.BadgesAwarded = result.BadgesAwarded;
                jsonResponse.ChallengesCompleted = result.ChallengesCompleted;
                jsonResponse.Since = "All Participants";
                jsonResponse.Success = true;
            }
            catch (Exception ex)
            {
                this.Log().Error("Status update error: {0}", ex.Message);
                jsonResponse.Success = false;
            }

            if (jsonResponse.Success)
            {
                try
                {
                    DateTime cacheUntil = DateTime.UtcNow.AddSeconds(30);
                    if (sessionTool.GetCache(context.Cache, CacheKey.Feed, tenantId) == null)
                    {
                        //this.Log().Debug("Caching status data until {0}",
                        //                 cacheUntil.ToLocalTime().ToLongTimeString());
                        string tenantCacheKey = sessionTool.GetTenantCacheKey(CacheKey.Status, tenantId);
                        context.Cache.Insert(tenantCacheKey,
                                             jsonResponse,
                                             null,
                                             cacheUntil,
                                             System.Web.Caching.Cache.NoSlidingExpiration,
                                             System.Web.Caching.CacheItemPriority.Default,
                                             null);
                    }
                }
                catch (Exception ex)
                {
                    this.Log().Error("Error caching status response: {0}", ex.Message);
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
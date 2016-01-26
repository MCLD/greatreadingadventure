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

namespace GRA.SRP.Handlers {
    public class JsonStatus : JsonBase {
        public int PointsEarned { get; set; }
        public int BadgesAwarded { get; set; }
        public int ChallengesCompleted { get; set; }
        public string Since { get; set; }
    }

    public class Status : IHttpHandler, IRequiresSessionState {
        public void ProcessRequest(HttpContext context) {
            var tenant = context.Session["TenantID"];
            int tenantId = tenant as int? ?? -1;
            if(tenantId == -1) {
                tenantId = Core.Utilities.Tenant.GetMasterID();
            }

            string cacheKey = string.Format("{0}.{1}", CacheKey.Status, tenantId);

            try {
                if(context.Cache[cacheKey] != null) {
                    var cachedJsonResponse = context.Cache[cacheKey] as JsonStatus;
                    if(cachedJsonResponse != null) {
                        context.Response.ContentType = "application/json";
                        context.Response.Write(JsonConvert.SerializeObject(cachedJsonResponse));
                        return;
                    }
                }
            } catch(Exception ex) {
                this.Log().Error("Error looking up status data in cache: {0}", ex.Message);
            }

            var jsonResponse = new JsonStatus();
            try {
                DateTime startingOn = DateTime.MinValue;
                if(!string.IsNullOrEmpty(context.Request.QueryString["StartingOn"])) {
                    DateTime.TryParse(context.Request.QueryString["StartingOn"], out startingOn);
                }
                
                ProgramStatusReport result = null;
                if(startingOn == DateTime.MinValue) {
                    result = new ProgramStatus(tenantId).CurrentStatus();
                } else {
                    result = new ProgramStatus(startingOn, tenantId).CurrentStatus();
                }

                jsonResponse.PointsEarned = result.PointsEarned;
                jsonResponse.BadgesAwarded = result.BadgesAwarded;
                jsonResponse.ChallengesCompleted = result.ChallengesCompleted;
                if(!string.IsNullOrEmpty(result.Since)) {
                    jsonResponse.Since = result.Since;
                } else {
                    jsonResponse.Since = "All Participants";
                }
                jsonResponse.Success = true;
            } catch(Exception ex) {
                this.Log().Error("Status update error: {0}", ex.Message);
                jsonResponse.Success = false;
            }

            if(jsonResponse.Success) {
                try {
                    DateTime cacheUntil = DateTime.UtcNow.AddSeconds(30);
                    if(context.Cache[cacheKey] == null) {
                        //this.Log().Debug("Caching status data until {0}",
                        //                 cacheUntil.ToLocalTime().ToLongTimeString());
                        context.Cache.Insert(cacheKey,
                                             jsonResponse,
                                             null,
                                             cacheUntil,
                                             System.Web.Caching.Cache.NoSlidingExpiration,
                                             System.Web.Caching.CacheItemPriority.Default,
                                             null);
                    }
                } catch(Exception ex) {
                    this.Log().Error("Error caching status response: {0}", ex.Message);
                }
            }


            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(jsonResponse));
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
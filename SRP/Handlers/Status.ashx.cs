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
            var jsonResponse = new JsonStatus();
            try {
                DateTime startingOn = DateTime.MinValue;
                if(!string.IsNullOrEmpty(context.Request.QueryString["StartingOn"])) {
                    DateTime.TryParse(context.Request.QueryString["StartingOn"], out startingOn);
                }
                ProgramStatusReport result = null;
                if(startingOn == DateTime.MinValue) {
                    result = new ProgramStatus().CurrentStatus();
                } else {
                    result = new ProgramStatus(startingOn).CurrentStatus();
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
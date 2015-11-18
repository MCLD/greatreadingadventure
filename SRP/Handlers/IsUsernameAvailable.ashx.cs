using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace GRA.SRP.Handlers {
    /// <summary>
    /// Summary description for IsUsernameAvailable
    /// </summary>
    public class JsonUsernameAvailable {
        public bool IsAvailable { get; set; }
    }
    public class IsUsernameAvailable : IHttpHandler, IRequiresSessionState {

        public void ProcessRequest(HttpContext context) {
            var jsonResponse = new JsonUsernameAvailable();
            string username = context.Request.QueryString["Username"];
            if(string.IsNullOrWhiteSpace(username)) {
                // no idea, then! we'll say it's available and deal with it when they submit
                jsonResponse.IsAvailable = true;
            } else if(username.Length > 50) {
                // too long, shouldn't even be here
                jsonResponse.IsAvailable = false;
            } else {
                var patron = DAL.Patron.GetObjectByUsername(context.Request.QueryString["Username"]);
                jsonResponse.IsAvailable = patron == null;
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
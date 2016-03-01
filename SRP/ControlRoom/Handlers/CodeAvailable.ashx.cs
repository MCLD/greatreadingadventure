using GRA.SRP.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace GRA.SRP.ControlRoom.Handlers
{
    /// <summary>
    /// Summary description for CodeAvailable
    /// </summary>
    public class CodeAvailable : IHttpHandler, IRequiresSessionState
    {
        public class JsonCodeAvailable
        {
            public bool Available { get; set; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var jsonResponse = new JsonCodeAvailable
            {
                Available = true
            };
            var code = context.Request.QueryString["code"];
            int eid = 0;
            bool haveEid = false;
            var eidString = context.Request.QueryString["eid"];
            if (!string.IsNullOrWhiteSpace(eidString) && int.TryParse(eidString, out eid)) {
                haveEid = true;
            }

            if (string.IsNullOrEmpty(code))
            {
                jsonResponse = null;
            }
            else
            {
                try
                {
                    int count = 0;
                    if (haveEid)
                    {
                        count = Event.GetEventCountByEventCode(eid, code);
                    }
                    else {
                        count = Event.GetEventCountByEventCode(code);
                    }
                    if (count != 0)
                    {
                        jsonResponse.Available = false;
                    }
                }
                catch (Exception ex)
                {
                    this.Log().Error("Error checking in CR for duplicate code: {0} - {1}",
                                     ex.Message,
                                     ex.StackTrace);
                    jsonResponse = null;
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
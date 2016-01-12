using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace GRA.SRP.Handlers {
    public class JsonEvent : JsonBase {
        public string Title { get; set; }
        public string When { get; set; }
        public string Where { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }

        public string CustomLabel1 { get; set; }
        public string CustomValue1 { get; set; }
        public string CustomLabel2 { get; set; }
        public string CustomValue2 { get; set; }
        public string CustomLabel3 { get; set; }
        public string CustomValue3 { get; set; }
    }

    /// <summary>
    /// Summary description for GetEventInfo
    /// </summary>
    public class GetEventInfo : IHttpHandler, IRequiresSessionState {

        public void ProcessRequest(HttpContext context) {
            var jsonResponse = new JsonEvent();
            if(context.Request.QueryString["EventId"] == null) {
                jsonResponse.Success = false;
                jsonResponse.ErrorMessage = "No event id provided.";
                this.Log().Error(string.Format("Event requested from {0}?{1} with no id.",
                                               context.Request.Url,
                                               context.Request.QueryString));
            } else {
                int eventId = 0;
                if(!int.TryParse(context.Request["EventId"].ToString(), out eventId)) {
                    jsonResponse.Success = false;
                    jsonResponse.ErrorMessage = "Invalid event id provided.";
                    this.Log().Error(string.Format("Requested event {0} from {1}?{2} - invalid id",
                                                   context.Request["EventId"].ToString(),
                                                   context.Request.Url,
                                                   context.Request.QueryString));
                } else {
                    DAL.Event e = new DAL.Event().FetchObject(eventId);
                    if(e == null) {
                        jsonResponse.Success = false;
                        jsonResponse.ErrorMessage = "Event not found.";
                        this.Log().Error(string.Format("Requested event {0} from {1}?{2} - not found",
                                                       eventId,
                                                       context.Request.Url,
                                                       context.Request.QueryString));
                    } else {
                        var cf = CustomEventFields.FetchObject();
                        jsonResponse.Success = true;
                        jsonResponse.Title = e.EventTitle;
                        jsonResponse.ShortDescription = e.ShortDescription;
                        jsonResponse.Description = context.Server.HtmlDecode(e.HTML);
                        jsonResponse.When = Event.DisplayEventDateTime(e);

                        if(e.BranchID > 0) {
                            var codeObject = DAL.Codes.FetchObject(e.BranchID);
                            if(codeObject != null) {
                                jsonResponse.Where = codeObject.Description;
                            }
                        }
                        if(!string.IsNullOrWhiteSpace(e.Custom1)
                           && !string.IsNullOrWhiteSpace(cf.Label1)) {
                            jsonResponse.CustomLabel1 = cf.Label1;
                            jsonResponse.CustomValue1 = e.Custom1;
                        }
                        if(!string.IsNullOrWhiteSpace(e.Custom2)
                           && !string.IsNullOrWhiteSpace(cf.Label2)) {
                            jsonResponse.CustomLabel2 = cf.Label2;
                            jsonResponse.CustomValue2 = e.Custom2;
                        }
                        if(!string.IsNullOrWhiteSpace(e.Custom3)
                           && !string.IsNullOrWhiteSpace(cf.Label3)) {
                            jsonResponse.CustomLabel3 = cf.Label3;
                            jsonResponse.CustomValue3 = e.Custom3;
                        }
                    }
                }
            }


            context.Response.ContentType = "application/json";
            var settings = new JsonSerializerSettings();
            settings.StringEscapeHandling = StringEscapeHandling.EscapeHtml;
            context.Response.Write(JsonConvert.SerializeObject(jsonResponse, settings));
        }

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}
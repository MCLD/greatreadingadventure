using Newtonsoft.Json;
using System.Web;
using System.Web.SessionState;

namespace GRA.SRP.ControlRoom.Handlers
{
    /// <summary>
    /// Summary description for UnreadMail
    /// </summary>
    public class UnreadMail : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(
                JsonConvert.SerializeObject(new Code.ControlRoom.Mail().UnreadCrMail(context)));
        }

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}
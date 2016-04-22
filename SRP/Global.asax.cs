using System;
using GRA.SRP.Core.Utilities;
using GRA;

namespace SRP
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            try
            {
                SRPUser.LogoffAll();
            }
            catch
            {
            }
            LoggingExtensions.Logging.Log.InitializeWith<LoggingExtensions.NLog.NLogLog>();
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
            try
            {
                SRPUser.LogoffAll();
            }
            catch
            {
            }
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            try
            {
                var ex = Server.GetLastError();

                this.Log().Error("Application error: {0} - {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception)
            {
                // couldn't log, do nothing
            }
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
            if (Session[SessionData.UserProfile.ToString()] != null)
            {
                SRPUser.Logoff(((SRPUser)Session[SessionData.UserProfile.ToString()]).Uid);
            }
        }
    }
}

using GRA.SRP.DAL;
using GRA.Tools;
using System;
using System.Web.SessionState;

namespace GRA.SRP {
    public class PatronSession {
        private HttpSessionState Session { get; set; }
        public PatronSession(HttpSessionState session) {
            this.Session = session;
        }
        public bool Establish(Patron patron) {
            try {
                Session["PatronLoggedIn"] = true;
                Session["Patron"] = patron;
                Session["ProgramID"] = patron.ProgID;
                Session["PatronProgramID"] = patron.ProgID;
                Session["CurrentProgramID"] = patron.ProgID;
                Session["TenantID"] = patron.TenID;
                Session[SessionKey.IsMasterAccount] = patron.IsMasterAccount;
                if(patron.IsMasterAccount) {
                    Session["MasterAcctPID"] = patron.PID;
                } else {
                    Session["MasterAcctPID"] = 0;
                }


                return true;
            } catch(Exception ex) {
                this.Log().Error(() => "Unable to establish patron session", ex);
                return false;
            }
        }

        public void Clear() {
            Session.Remove("PatronLoggedIn");
            Session.Remove("Patron");
            Session.Remove("ProgramID");
            Session.Remove("PatronProgramID");
            Session.Remove("CurrentProgramID");
            Session.Remove(SessionKey.IsMasterAccount);
            Session.Remove("MasterAcctPID");
        }
    }
}
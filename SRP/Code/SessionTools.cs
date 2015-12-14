using GRA.SRP.DAL;
using GRA.Tools;
using System;
using System.Web.SessionState;

namespace GRA.SRP {
    public class SessionTools {
        private HttpSessionState Session { get; set; }
        public SessionTools(HttpSessionState session) {
            this.Session = session;
        }
        public bool EstablishPatron(Patron patron) {
            try {
                Session[SessionKey.Patron] = patron;
                Session["ProgramID"] = patron.ProgID;
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

        public void ClearPatron() {
            Session.Remove(SessionKey.Patron);
            Session.Remove("ProgramID");
            Session.Remove(SessionKey.IsMasterAccount);
            Session.Remove("MasterAcctPID");
        }

        public void EarnedBadges(object badgeIds) {
            Session[SessionKey.EarnedBadges] = badgeIds;
            Session[SessionKey.RefreshBadgeList] = true;
        }

        public void ClearEarnedBadges() {
            Session.Remove(SessionKey.EarnedBadges);
        }

        public void ClearRefreshBadgeList() {
            Session.Remove(SessionKey.RefreshBadgeList);
        }

        public void AlertPatron(string message,
                                string patronMessageLevel = null,
                                string glyphicon = null) {
            Session[SessionKey.PatronMessage] = message;
            if(patronMessageLevel != null) {
                Session[SessionKey.PatronMessageLevel] = patronMessageLevel;
            }
            if(!string.IsNullOrEmpty(glyphicon)) {
                Session[SessionKey.PatronMessageGlyphicon] = glyphicon;
            }
        }
        public void ClearPatronAlert() {
            Session.Remove(SessionKey.PatronMessage);
            Session.Remove(SessionKey.PatronMessageLevel);
            Session.Remove(SessionKey.PatronMessageGlyphicon);
        }
    }
}
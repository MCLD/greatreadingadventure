using GRA.SRP.DAL;
using GRA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace GRA.SRP.Code {
    public class FamilyTools {
        /// <summary>
        /// Usess the request or session SA value along with the session MasterAcctPID value to
        /// determine if the logged in patron can impersonate the patron with the SA patron id.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public FamilyRelationship ValidImpersonation(HttpRequest request, HttpSessionState session) {
            string sa;
            if(string.IsNullOrEmpty(request[SessionKey.SA])
               && (session[SessionKey.SA] == null || string.IsNullOrEmpty(session[SessionKey.SA].ToString()))) {
                return null;
            }
            if(!string.IsNullOrEmpty(request[SessionKey.SA])) {
                sa = request[SessionKey.SA];
                session[SessionKey.SA] = sa;
            } else {
                sa = session[SessionKey.SA].ToString();
            }

            var parent = Patron.FetchObject((int)session[SessionKey.MasterAcctPID]);
            if(!parent.IsMasterAccount
               || !Patron.CanManageSubAccount(parent.PID, int.Parse(sa))) {
                return null;
            }

            return new FamilyRelationship {
                PatronId = int.Parse(sa),
                ParentPatronId = parent.PID
            };
        }
    }
}
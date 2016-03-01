using GRA.SRP.DAL;
using GRA.Tools;
using System;
using System.Web.Caching;
using System.Web.SessionState;

namespace GRA.SRP
{
    public class SessionTools
    {
        private HttpSessionState Session { get; set; }
        public SessionTools(HttpSessionState session)
        {
            this.Session = session;
        }
        public bool EstablishPatron(Patron patron)
        {
            try
            {
                Session[SessionKey.Patron] = patron;
                Session["ProgramID"] = patron.ProgID;
                Session["TenantID"] = patron.TenID;
                Session[SessionKey.IsMasterAccount] = patron.IsMasterAccount;
                if (patron.IsMasterAccount)
                {
                    Session["MasterAcctPID"] = patron.PID;
                }
                else {
                    Session["MasterAcctPID"] = 0;
                }
                return true;
            }
            catch (Exception ex)
            {
                this.Log().Error(() => "Unable to establish patron session", ex);
                return false;
            }
        }

        public void ClearPatron()
        {
            Session.Remove(SessionKey.Patron);
            Session.Remove("ProgramID");
            Session.Remove(SessionKey.IsMasterAccount);
            Session.Remove("MasterAcctPID");
        }

        public void EarnedBadges(object badgeIds)
        {
            Session[SessionKey.EarnedBadges] = badgeIds;
            Session[SessionKey.RefreshBadgeList] = true;
        }

        public void ClearEarnedBadges()
        {
            Session.Remove(SessionKey.EarnedBadges);
        }

        public void ClearRefreshBadgeList()
        {
            Session.Remove(SessionKey.RefreshBadgeList);
        }

        public void AlertPatron(string message,
                                string patronMessageLevel = null,
                                string glyphicon = null)
        {
            Session[SessionKey.PatronMessage] = message;
            if (patronMessageLevel != null)
            {
                Session[SessionKey.PatronMessageLevel] = patronMessageLevel;
            }
            if (!string.IsNullOrEmpty(glyphicon))
            {
                Session[SessionKey.PatronMessageGlyphicon] = glyphicon;
            }
        }
        public void ClearPatronAlert()
        {
            Session.Remove(SessionKey.PatronMessage);
            Session.Remove(SessionKey.PatronMessageLevel);
            Session.Remove(SessionKey.PatronMessageGlyphicon);
        }

        public string GetTenantCacheKey(string cacheKey, int? tenantId = null)
        {
            int lookupTenantId = tenantId ?? -1;
            if (tenantId == null)
            {
                var tenantObject = Session[SessionKey.TenantID];
                if (tenantObject != null && tenantObject.ToString().Length != 0)
                {
                    lookupTenantId = (int)tenantObject;
                }
            }
            return string.Format("{0}.{1}", cacheKey, lookupTenantId);
        }

        public void RemoveCache(Cache cache, string cacheKey, int? tenantId = null)
        {
            string key = GetTenantCacheKey(cacheKey, tenantId);
            if (cache[key] != null)
            {
                cache.Remove(key);
            }
        }

        public object GetCache(Cache cache, string cacheKey, int? tenantId = null)
        {
            string key = GetTenantCacheKey(cacheKey, tenantId);
            return cache[key];
        }

        public void SetCache(Cache cache, string cacheKey, object value, int? tenantId = null)
        {
            string key = GetTenantCacheKey(cacheKey, tenantId);
            cache[key] = value;
        }
    }
}
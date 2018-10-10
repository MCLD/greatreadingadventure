using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA
{
    public static class CacheKey
    {
        public static readonly string CurrentStats = "CurrentStats";
        public static readonly string Sites = "Sites";
        public static readonly string SiteSettings = "SiteSettings";
        public static readonly string UnhandledMailCount = "UnhandledMailCount";
        public static readonly string UserUnreadMailCount = "UserUnreadMailCount";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRA.Tools {
    public static class SessionKey {
        public const string PatronMessage = "PatMsg";
        public const string PatronMessageGlyphicon = "PatMsgGly";
        public const string PatronMessageLevel = "PatMsgLev";
        public const string IsMasterAccount = "IsMasterAcct";
        public const string RequestedPath = "ReqPath";
        public const string AdventuresActive = "AdvAct";
        public const string ChallengesActive = "ChalAct";
        public const string OffersActive = "OffAct";
        public const string EventsActive = "EvAct";
        public const string BadgesActive = "BadAct";
        public const string Patron = "Patron";
        public const string EarnedBadges = "ErnBdg";
    }

    public static class PatronMessageLevels {
        public const string Success = "success";
        public const string Info = "info";
        public const string Warning = "warning";
        public const string Danger = "danger";
    }
}

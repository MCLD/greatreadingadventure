using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Model
{
    public enum SiteStage
    {
        Unknown,
        BeforeRegistration,
        RegistrationOpen,
        ProgramOpen,
        ProgramEnded,
        AccessClosed
    }
}

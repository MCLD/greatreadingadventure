using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.Base
{
    public interface ISitePathValidator
    {
        bool IsValid(string sitePath);
    }
}

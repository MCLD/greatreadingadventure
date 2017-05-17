using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Abstract
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}

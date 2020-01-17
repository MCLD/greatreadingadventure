using System;

namespace GRA.Abstract
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}

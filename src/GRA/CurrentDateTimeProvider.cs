using System;

namespace GRA
{
    public class CurrentDateTimeProvider : Abstract.IDateTimeProvider
    {
        public DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}

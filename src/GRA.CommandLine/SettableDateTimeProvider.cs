using System;
using GRA.Abstract;

namespace GRA.CommandLine
{
    public class SettableDateTimeProvider : IDateTimeProvider
    {
        private DateTime _setDateTime;
        public DateTime Now
        {
            get
            {
                return _setDateTime;
            }
        }

        public SettableDateTimeProvider()
        {
            _setDateTime = DateTime.Now;
        }

        public void SetDateTime(DateTime dateTime)
        {
            _setDateTime = dateTime;
        }
    }
}

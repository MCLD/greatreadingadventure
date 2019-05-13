using System;

namespace GRA
{
    public class GraFatalException : GraException
    {
        public GraFatalException() : base()
        {
        }

        public GraFatalException(string message) : base(message)
        {
        }

        public GraFatalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

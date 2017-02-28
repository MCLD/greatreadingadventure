using System;

namespace GRA
{
    public class GraException : Exception
    {
        public GraException() { }

        public GraException(string message) : base(message) { }

        public GraException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}

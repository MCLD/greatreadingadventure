using System;

namespace GRA
{
    public class GraDbUpdateException : GraException
    {
        public GraDbUpdateException()
        {
        }

        public GraDbUpdateException(string message) : base(message)
        {
        }

        public GraDbUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

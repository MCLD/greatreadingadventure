using System;
using System.Collections.Generic;
using System.Text;

namespace GRA
{
    public class GraPasswordValidationException : GraException
    {
        public GraPasswordValidationException() { }

        public GraPasswordValidationException(string message) : base(message) { }

        public GraPasswordValidationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}

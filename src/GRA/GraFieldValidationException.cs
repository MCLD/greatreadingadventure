using System.Collections.Generic;

namespace GRA
{
    public class GraFieldValidationException : GraException
    {
        public IDictionary<string, string> FieldValidationErrors { get; }
        public GraFieldValidationException(IDictionary<string, string> fieldValidationErrors)
        {
            FieldValidationErrors = fieldValidationErrors;
        }

        public GraFieldValidationException()
        {
        }

        public GraFieldValidationException(string message) : base(message)
        {
        }

        public GraFieldValidationException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

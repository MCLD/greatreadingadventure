using System.Linq;

namespace GRA
{
    public class GraFieldValidationException : GraException
    {
        public ILookup<string, string> FieldValidationErrors { get; }
        public GraFieldValidationException(ILookup<string, string> fieldValidationErrors)
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

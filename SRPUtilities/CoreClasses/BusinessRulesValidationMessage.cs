using System;

namespace GRA.SRP.Core.Utilities
{
    [Serializable]
    public partial class BusinessRulesValidationMessage
    {
        public BusinessRulesValidationMessage(string fieldName, string fieldNameDisplay, string errorMessage, BusinessRulesValidationCode errorCode)
        {
            FieldName = fieldName;
            FieldNameDisplay = fieldNameDisplay;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public string FieldName { get; set; }
        public string FieldNameDisplay { get; set; }
        public string ErrorMessage { get; set; }
        public BusinessRulesValidationCode ErrorCode { get; set; }
    }
}
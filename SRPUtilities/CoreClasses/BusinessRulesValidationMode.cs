using System;

namespace GRA.SRP.Core.Utilities
{
    [Serializable]
    public enum BusinessRulesValidationMode
    {
        INSERT = 1,
        UPDATE = 2,
        DELETE = 3,
        CUSTOM = 4,
        GENERIC = 5,
        OTHER = 6
    }




}
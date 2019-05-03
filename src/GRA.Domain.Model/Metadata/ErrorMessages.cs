namespace GRA.Domain.Model
{
    public static class ErrorMessages
    {
        public const string Field = "The {0} field is required.";
        public const string FieldBranch = "You must select a branch.";
        public const string FieldProgram = "You must select a program.";
        public const string FieldSystem = "You must select a system.";
        public const string MailUnableToDelete = "There was an issue deleting that mail item.";
        public const string MailUnableToRead = "Unable to read mail: {0}";
        public const string MailUnableToReply = "Unable to reply to mail: {0}";
        public const string MaxLength = "The field {0} must be a string or array type with a maximum length of '{1}'.";
        public const string MinLength = "The field {0} must be a string or array type with a minimum length of '{1}'.";
        public const string Selection = "Please select a value for {0}.";
    }
}

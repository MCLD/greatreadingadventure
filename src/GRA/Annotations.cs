namespace GRA
{
    public static class Annotations
    {
        public static class Info
        {
            public const string BetterSuitedOption = "This program may be better-suited to your age:";
            public const string Goal = "The goal of this program is {0} points.";
            public const string LetYouKnowWhen = "Thanks! We'll let you know when you can join the program.";
            public const string PasswordRecoverySent = "A password recovery email has been sent to the email for username '{0}'.";
            public const string PasswordResetFor = "Password reset for: {0}";
            public const string ProgramEnded = "{0} has ended, please join us next time!";
            public const string RegistrationNotOpenYet = "Registration for {0} is not open yet.";
            public const string UsernameIsAvailable = "That username is available!";
            public const string UsernameListSent = "A list of usernames associated with the email address '{0}' has been sent.";
            public const string YouCanJoinOn = "You can join {0} on {1}.";
        }

        public static class Title
        {
            public const string Join = "Join";
            public const string JoinNow = "{0} - Join Now!";
            public const string ForgotPassword = "Forgot Password";
            public const string SignIn = "Sign in";
            public const string SignInTo = "Sign in to {0}";
            public const string UsernameRecovery = "Username Recovery";
        }

        public static class Required
        {
            public const string Branch = "Please select a branch.";
            public const string EmailForSubscription = "To receive email updates please supply an email address to send them to.";
            public const string Field = "The {0} field is required.";
            public const string JavaScriptWarning = "This website relies on JavaScript. Please enable JavaScript in your browser to get the best experience.";
            public const string ProgramSelection = "Please select a program.";
            public const string Selection = "Please select a value for {0}.";
            public const string System = "Please select a system.";
        }

        public static class Validate
        {
            public const string BookTitle = "When providing an author of a book, please also provide a title.";
            public const string Branch = "The branch you've selected is not valid, please select another branch.";
            public const string CouldNotCreate = "Could not create your account: {0}";
            public const string CouldNotLog = "Sorry, we couldn't log your activity: {0}";
            public const string CouldNotRecover = "Could not recover username(s): {0}";
            public const string Email = "The {0} field is not a valid e-mail address.";
            public const string EmailConfigured = "Username '{0}' does not have an email address configured.";
            public const string EmailPhoneNeededForPrizes = "Email and Phone Number are not required however at least one is needed in order to be eligible for prizes.";
            public const string EmailSubscription = "Please let us know if you would like to receive emails throughout the program.";
            public const string FirstTime = "Please let us know if this is your first time participating in the program.";
            public const string MaxLength = "The field {0} must be a string or array type with a maximum length of '{1}'.";
            public const string MinLength = "The field {0} must be a string or array type with a minimum length of '{1}'.";
            public const string NotOpen = "The program is not accepting registrations at this time.";
            public const string NotOpenActivity = "The program is not open for activity at this time.";
            public const string NotOpenSignins = "The program is not accepting sign-ins at this time.";
            public const string Password = "The provided password is incorrect.";
            public const string PasswordIssue = "Please correct the issues with your password.";
            public const string PasswordLength = "The password you've provided is too short, it must be at least {0} characters long.";
            public const string PasswordRequirement = "Your password must be at least 6 characters long and include a number.";
            public const string PasswordRequiresDigit = "You must include a number in your password.";
            public const string PasswordRequiresLowercase = "You must include a lowercase letter in your password.";
            public const string PasswordRequiresUppercase = "You must include an uppercase letter in your password.";
            public const string PasswordRequiresSymbol = "You must include a special character in your password.";
            public const string PasswordsMatch = "Password and password confirmation do not match.";
            public const string Permission = "Permission denied.";
            public const string Phone = "The {0} field is not a valid phone number.";
            public const string Program = "The program you've selected is not valid, please select another program.";
            public const string School = "The school you've selected is not valid, please select another school.";
            public const string System = "The system you've selected is not valid, please select another system.";
            public const string Token = "Password reset token '{0}' is not valid, please start the password reset process over.";
            public const string TokenExpired = "Password reset token '{0}' has expired, please start the password reset process over.";
            public const string UnableToReset = "Unable to reset password: {0}";
            public const string Username = "Could not find username '{0}'.";
            public const string UsernamePasswordMismatch = "The username and password entered do not match.";
            public const string UsernameTaken = "Someone has already chosen that username, please try another.";
            public const string WholeNumber = "Please enter a whole number greater than 0.";
        }
    }
}

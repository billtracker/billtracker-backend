namespace BillTracker.Identity
{
    public static class IdentityErrors
    {
        public const string InvalidUser = "Invalid email address, user name or password.";
        public const string EmailTaken = "Email address or user name is taken.";

        public const string InvalidRefreshToken = "Invalid refresh token.";
    }
}

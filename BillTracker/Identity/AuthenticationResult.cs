using System;

namespace BillTracker.Identity
{
    public class AuthenticationResult
    {
        public AuthenticationResult(string accessToken, string refreshToken, DateTimeOffset expiresAt)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresAt = expiresAt;
        }

        public string AccessToken { get; }
        public string RefreshToken { get; }
        public DateTimeOffset ExpiresAt { get; }
    }
}

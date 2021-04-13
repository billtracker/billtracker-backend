using System;

namespace BillTracker.Identity
{
    public record AuthenticationResult
    {
        public string AccessToken { get; init; }

        public string RefreshToken { get; init; }

        public DateTimeOffset ExpiresAt { get; init; }
    }
}

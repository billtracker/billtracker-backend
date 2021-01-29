using System;

namespace BillTracker.Identity
{
    public class JwtTokenResult
    {
        public JwtTokenResult(string accessToken, DateTimeOffset validTo)
        {
            AccessToken = accessToken;
            ValidTo = validTo;
        }

        public string AccessToken { get; }
        public DateTimeOffset ValidTo { get; }
    }
}

using System;

namespace BillTracker.Identity
{
    public class IdentityConfiguration
    {
        public const string SectionName = "Identity";

        public string Secret { get; set; }

        public string Issuer { get; set; }

        public TimeSpan? AccessTokenValidity { get; set; }
    }
}

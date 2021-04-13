namespace BillTracker.Api.Recaptcha
{
    internal record RecaptchaConfiguration
    {
        public const string SectionName = "Recaptcha";

        public bool UseRecaptcha { get; init; }

        public string Secret { get; init; }
    }
}

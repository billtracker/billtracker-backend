namespace BillTracker.Api.Recaptcha
{
    internal record RecaptchaConfiguration
    {
        public const string SectionName = "Recaptcha";

        public string Secret { get; init; }

        public bool UseRecaptcha
        {
            get => !string.IsNullOrEmpty(Secret);
        }
    }
}

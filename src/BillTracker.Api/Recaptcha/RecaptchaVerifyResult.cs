namespace BillTracker.Api.Recaptcha
{
    internal record RecaptchaVerifyResult
    {
        public bool Success { get; init; }
    }
}

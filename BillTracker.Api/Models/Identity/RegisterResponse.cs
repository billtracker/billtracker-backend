namespace BillTracker.Api.Models.Identity
{
    public record RegisterResponse
    {
        public string EmailAddress { get; init; }

        public string UserName { get; init; }
    }
}

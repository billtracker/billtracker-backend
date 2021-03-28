namespace BillTracker
{
    internal class AzureConfiguration
    {
        public const string SectionName = "Azure";

        public string BlobStorageConnectionString { get; set; }
    }
}

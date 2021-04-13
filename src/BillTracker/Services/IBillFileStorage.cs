using System;
using System.IO;
using System.Threading.Tasks;

namespace BillTracker.Services
{
    public interface IBillFileStorage
    {
        Task<Uri> Upload(UploadBillBlob parameters);
    }

    public class UploadBillBlob
    {
        public UploadBillBlob(Stream file, Guid userId, Guid aggregateId, string fileName)
        {
            File = file;
            UserId = userId;
            AggregateId = aggregateId;
            FileName = fileName;
        }

        public Stream File { get; }

        public Guid UserId { get; }

        public Guid AggregateId { get; }

        public string FileName { get; }
    }
}

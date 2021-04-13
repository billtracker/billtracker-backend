using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Models;
using BillTracker.Services;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Commands
{
    public class UploadBillFile
    {
        public const string BillFileIsEmpty = "Sent file is empty.";
        public const string BillFileIsTooBig = "Sent file is too big.";

        private const int MaximumBillFileSizeInMB = 10;

        private readonly BillTrackerContext _context;
        private readonly IBillFileStorage _billFileStorage;

        public UploadBillFile(BillTrackerContext context, IBillFileStorage billFileStorage)
        {
            _context = context;
            _billFileStorage = billFileStorage;
        }

        public async Task<ResultOrError<ExpenseBillFileModel>> Handle(AddBillFileParameters parameters)
        {
            if (parameters.File == null || parameters.File.Length == 0)
            {
                return BillFileIsEmpty;
            }

            if (parameters.File.Length > MaximumBillFileSizeInMB * 1000000)
            {
                return BillFileIsTooBig;
            }

            var aggregate = await _context.ExpensesAggregates
                .Include(x => x.ExpenseBillFiles)
                .SingleOrDefaultAsync(x => x.Id == parameters.AggregateId && x.UserId == parameters.UserId);
            if (aggregate == null)
            {
                return CommonErrors.ExpenseAggregateNotFound;
            }

            var fileName = $"{aggregate.ExpenseBillFiles.Count()}-{parameters.FileName}";
            var fileUri = await _billFileStorage.Upload(
                new UploadBillBlob(
                    file: parameters.File,
                    userId: parameters.UserId,
                    aggregateId: parameters.AggregateId,
                    fileName: fileName));

            var entity = ExpenseBillFile.Create(parameters.AggregateId, fileName, fileUri);

            await _context.ExpenseBills.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity.ToModel();
        }
    }

    public class AddBillFileParameters
    {
        public AddBillFileParameters(Guid userId, Guid aggregateId, Stream file, string fileName)
        {
            UserId = userId;
            AggregateId = aggregateId;
            File = file;
            FileName = fileName;
        }

        public Guid UserId { get; }

        public Guid AggregateId { get; }

        public Stream File { get; }

        public string FileName { get; }
    }
}

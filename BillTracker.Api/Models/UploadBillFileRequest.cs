using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BillTracker.Api.Models
{
    public class UploadBillFileRequest
    {
        [Required]
        public Guid? AggregateId { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}

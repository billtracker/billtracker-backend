﻿using System;

namespace BillTracker.Api.Models
{
    public class SaveExpenseAggregateRequest
    {
        public Guid? Id { get; set; }

        public DateTimeOffset? AddedDate { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; }
    }
}
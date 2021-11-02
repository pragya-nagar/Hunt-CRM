using System;
using System.Collections.Generic;

namespace Synergy.CRM.Models
{
    public class ReminderFilterArgs
    {
        public Guid Id { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime SheduledDate { get; set; }

        public IEnumerable<Guid> LeadIds { get; set; }

        public IEnumerable<Guid> OpportunityIds { get; set; }

        public bool? IsUpcoming { get; set; }

        public Guid MyReminderUserId { get; set; }

        public bool? IsExpiredNotMoreThanThirtyDays { get; set; }
    }
}

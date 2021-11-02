using System;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Models.Commands.Reminder
{
    public class ReminderCreateCommand : Command
    {
        public Guid? UserId { get; set; }

        public Guid LeadId { get; set; }

        public Guid OpportunityId { get; set; }

        public Guid ContactId { get; set; }

        public string Description { get; set; }

        public DateTime SheduledDate { get; set; }

        public TimeSpan SheduledTime { get; set; }

        public bool? IsPushNotification { get; set; }

        public bool? IsEmailNotification { get; set; }

        public int? Status { get; set; }

        public bool? IsRead { get; set; }
    }
}

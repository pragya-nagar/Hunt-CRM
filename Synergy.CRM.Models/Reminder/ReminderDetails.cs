using System;
using Synergy.Common.Domain.Models.Abstracts;
using Synergy.Common.Domain.Models.Common;

namespace Synergy.CRM.Models
{
    public class ReminderDetails : IResultModel
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public Guid LeadId { get; set; }

        public Guid OpportunityId { get; set; }

        public Guid ContactId { get; set; }

        public string Description { get; set; }

        public DateTime SheduledDate { get; set; }

        public TimeSpan SheduledTime { get; set; }

        public bool? IsPushNotification { get; set; }

        public bool? IsEmailNotification { get; set; }

        public string NotificationType { get; set; }

        public string Status { get; set; }

        public bool? IsRead { get; set; }

        public FastEntityModel<Guid> User { get; set; }

        public FastEntityModel<Guid> Contact { get; set; }
    }
}

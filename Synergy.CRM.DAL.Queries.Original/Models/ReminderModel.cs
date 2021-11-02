using System;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Abstractions.Models;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class ReminderModel : AuditModel, IModel
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

        public int? Status { get; set; }

        public bool? IsRead { get; set; }

        public string NewStatus { get; set; }

        public string NotificationType { get; set; }

        public FastEntityModel<Guid> User { get; set; }

        public FastEntityModel<Guid> Contact { get; set; }
    }
}

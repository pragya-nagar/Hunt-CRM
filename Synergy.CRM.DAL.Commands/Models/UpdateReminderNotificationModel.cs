using System;

namespace Synergy.CRM.DAL.Commands.Models
{
    public class UpdateReminderNotificationModel
    {
        public Guid Id { get; set; }

        public bool? IsPushNotification { get; set; }

        public bool? IsEmailNotification { get; set; }

        public int? Status { get; set; }
    }
}

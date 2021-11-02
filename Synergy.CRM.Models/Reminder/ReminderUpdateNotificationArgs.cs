using System;
using System.Collections.Generic;
using System.Text;

namespace Synergy.CRM.Models.Reminder
{
    public class ReminderUpdateNotificationArgs
    {
        public bool? IsPushNotification { get; set; }

        public bool? IsEmailNotification { get; set; }

        public int? Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Models.Commands.Reminder
{
    public class ReminderUpdateNotificationCommand : Command
    {
        public bool? IsPushNotification { get; set; }

        public bool? IsEmailNotification { get; set; }

        public int? Status { get; set; }
    }
}

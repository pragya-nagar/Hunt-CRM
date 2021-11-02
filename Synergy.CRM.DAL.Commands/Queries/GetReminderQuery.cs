using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.CRM.DAL.Commands.Models.Results.Opportunity;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;
using Synergy.DataAccess.Context;

namespace Synergy.CRM.DAL.Commands.Queries
{
    public class GetReminderQuery : SingleQuery<Guid, CreateReminderModel>
    {
        private readonly ISynergyContext _context;

        public GetReminderQuery(ISynergyContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override async Task<CreateReminderModel> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var reminder = await _context.Reminder.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedOn == null, cancellationToken)
                .ConfigureAwait(false);

            return new CreateReminderModel
            {
                Id = reminder.Id,
                IsPushNotification = reminder.IsPushNotification,
                IsEmailNotification = reminder.IsEmailNotification,
                Status = reminder.Status,
                SheduledDate = reminder.SheduledDate,
                SheduledTime = reminder.SheduledTime,
            };
        }
    }
}

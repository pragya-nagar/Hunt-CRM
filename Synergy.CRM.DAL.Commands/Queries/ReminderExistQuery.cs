using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;
using Synergy.DataAccess.Context;

namespace Synergy.CRM.DAL.Commands.Queries
{
    public class ReminderExistQuery : SingleQuery<Guid, bool>
    {
        private readonly ISynergyContext _context;

        public ReminderExistQuery(ISynergyContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override async Task<bool> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await this._context.Reminder.AsNoTracking().AnyAsync(x => x.Id == id && x.DeletedOn == null, cancellationToken).ConfigureAwait(false);
        }
    }
}

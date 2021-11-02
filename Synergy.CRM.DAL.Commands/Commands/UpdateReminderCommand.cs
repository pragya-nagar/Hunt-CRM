using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Abstractions.Commands;
using Synergy.DataAccess.Context;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.Commands
{
    public class UpdateReminderCommand : IUpdateReminderCommand
    {
        private readonly IMapper _mapper;
        private readonly ISynergyContext _context;

        public UpdateReminderCommand(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public void Dispatch(UpdateReminderModel reminder, Guid userId)
        {
            this.UpdateEntity(reminder, userId);
            this._context.SaveChanges();
        }

        public Task<int> DispatchAsync(UpdateReminderModel reminder, Guid userId, CancellationToken cancellationToken = default)
        {
            this.UpdateEntity(reminder, userId);
            return this._context.SaveChangesAsync(cancellationToken);
        }

        private void UpdateEntity(UpdateReminderModel reminder, Guid userId)
        {
            var reminderEntity = this._context.Reminder.Single(x => x.Id == reminder.Id).OnModifyAudit(userId);
            this._mapper.Map(reminder, reminderEntity);
            this._context.Reminder.Update(reminderEntity);
        }
    }
}

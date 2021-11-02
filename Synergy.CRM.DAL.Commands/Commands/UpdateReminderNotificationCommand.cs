using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Abstractions.Commands;
using Synergy.DataAccess.Context;

namespace Synergy.CRM.DAL.Commands.Commands
{
    public class UpdateReminderNotificationCommand : IUpdateReminderNotificationCommand
    {
        private readonly IMapper _mapper;
        private readonly ISynergyContext _context;

        public UpdateReminderNotificationCommand(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public void Dispatch(UpdateReminderNotificationModel notification, Guid userId)
        {
            this.UpdateEntity(notification, userId);
            this._context.SaveChanges();
        }

        public Task<int> DispatchAsync(UpdateReminderNotificationModel notification, Guid userId, CancellationToken cancellationToken = default)
        {
            this.UpdateEntity(notification, userId);
            return this._context.SaveChangesAsync(cancellationToken);
        }

        private void UpdateEntity(UpdateReminderNotificationModel notification, Guid userId)
        {
            var notificationEntity = this._context.Reminder.Single(x => x.Id == notification.Id).OnModifyAudit(userId);
            this._mapper.Map(notification, notificationEntity);
            this._context.Reminder.Update(notificationEntity);
        }
    }
}

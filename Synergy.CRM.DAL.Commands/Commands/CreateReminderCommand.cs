using System;
using System.Collections.Generic;
using System.Text;
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
   public class CreateReminderCommand : ICreateReminderCommand
   {
        private readonly IMapper _mapper;
        private readonly ISynergyContext _context;

        public CreateReminderCommand(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public Task<int> DispatchAsync(CreateReminderModel reminder, Guid userId, CancellationToken cancellationToken = default)
        {
            this.AddEntity(reminder, userId);
            return this._context.SaveChangesAsync(cancellationToken);
        }

        public void Dispatch(CreateReminderModel entity, Guid userId)
        {
            this.AddEntity(entity, userId);
            this._context.SaveChanges();
        }

        private void AddEntity(CreateReminderModel reminder, Guid userId)
        {
            var entity = this._mapper.Map<Reminder>(reminder).OnCreateAudit(userId);
            this._context.Reminder.Add(entity);
        }
    }
}

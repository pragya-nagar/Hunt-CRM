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
    public class UpdateContactCommand : IUpdateContactCommand
    {
        private IMapper _mapper;
        private ISynergyContext _context;

        public UpdateContactCommand(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public void Dispatch(UpdateContactModel contact, Guid userId)
        {
            this.UpdateEntity(contact, userId);
            this._context.SaveChanges();
        }

        public Task<int> DispatchAsync(UpdateContactModel contact, Guid userId, CancellationToken cancellationToken = default)
        {
            this.UpdateEntity(contact, userId);
            return this._context.SaveChangesAsync(cancellationToken);
        }

        private void UpdateEntity(UpdateContactModel contact, Guid userId)
        {
            var contactEntity = this._context.Contact.Single(x => x.Id == contact.Id).OnModifyAudit(userId);
            this._mapper.Map(contact, contactEntity);
            this._context.Contact.Update(contactEntity);
        }
    }
}

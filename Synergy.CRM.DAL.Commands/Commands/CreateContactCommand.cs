using System;
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
    public class CreateContactCommand : ICreateContactCommand
    {
        private IMapper _mapper;
        private ISynergyContext _context;

        public CreateContactCommand(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public void Dispatch(CreateContactModel contact, Guid userId)
        {
            this.AddEntity(contact, userId);
            this._context.SaveChanges();
        }

        public Task<int> DispatchAsync(CreateContactModel contact, Guid userId, CancellationToken cancellationToken = default)
        {
            this.AddEntity(contact, userId);
            return this._context.SaveChangesAsync(cancellationToken);
        }

        private void AddEntity(CreateContactModel contact, Guid userId)
        {
            var entity = this._mapper.Map<Contact>(contact).OnCreateAudit(userId);
            this._context.Contact.Add(entity);
        }
    }
}

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
    public class CreateLeadCommentCommand : ICreateLeadCommentCommand
    {
        private IMapper _mapper;
        private ISynergyContext _context;

        public CreateLeadCommentCommand(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public void Dispatch(CreateLeadCommentModel comment, Guid userId)
        {
            this.AddEntity(comment, userId);
            this._context.SaveChanges();
        }

        public Task<int> DispatchAsync(CreateLeadCommentModel comment, Guid userId, CancellationToken cancellationToken = default)
        {
            this.AddEntity(comment, userId);
            return this._context.SaveChangesAsync(cancellationToken);
        }

        private void AddEntity(CreateLeadCommentModel comment, Guid userId)
        {
            var entity = this._mapper.Map<LeadComment>(comment).OnCreateAudit(userId);
            entity.AuthorId = userId;
            entity.CommentDate = entity.CreatedOn;

            this._context.LeadComment.Add(entity);
        }
    }
}

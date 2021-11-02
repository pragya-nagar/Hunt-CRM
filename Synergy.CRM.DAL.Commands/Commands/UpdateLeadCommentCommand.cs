using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synergy.Common.Exceptions;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Abstractions.Commands;
using Synergy.DataAccess.Context;

namespace Synergy.CRM.DAL.Commands.Commands
{
    public class UpdateLeadCommentCommand : IUpdateLeadCommentCommand
    {
        private readonly ISynergyContext _context;

        public UpdateLeadCommentCommand(ISynergyContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Dispatch(UpdateLeadCommentModel comment, Guid userId)
        {
            this.DispatchAsync(comment, userId).GetAwaiter().GetResult();
        }

        public async Task<int> DispatchAsync(UpdateLeadCommentModel comment, Guid userId, CancellationToken cancellationToken = default)
        {
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            var entity = await this._context.LeadComment
                .FirstOrDefaultAsync(x => x.Id == comment.Id, cancellationToken)
                .ConfigureAwait(false);

            if (entity == null)
            {
                throw new NotFoundException();
            }

            entity.Comment = comment.Comment;
            entity.OnModifyAudit(userId);

            return await this._context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
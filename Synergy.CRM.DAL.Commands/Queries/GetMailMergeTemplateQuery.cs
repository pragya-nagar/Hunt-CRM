using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Synergy.CRM.DAL.Commands.Models.Results.MailMerge;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;
using Synergy.DataAccess.Context;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.Queries
{
    public class GetMailMergeTemplateQuery : SingleQuery<Guid, MailMergeTemplateModel>
    {
        private readonly ISynergyContext _synergyContext;

        public GetMailMergeTemplateQuery(ISynergyContext synergyContext)
        {
            this._synergyContext = synergyContext ?? throw new ArgumentNullException(nameof(synergyContext));
        }

        public override async Task<MailMergeTemplateModel> ExecuteAsync(Guid templateId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this._synergyContext.GetQueryable<TemplateFile>().Select(x => new MailMergeTemplateModel
            {
                Id = x.Id,
                GroupingType = x.TemplateType.GroupingType,
                FileId = x.FileId,
            }).FirstOrDefaultAsync(x => x.Id == templateId, cancellationToken).ConfigureAwait(false);
        }
    }
}

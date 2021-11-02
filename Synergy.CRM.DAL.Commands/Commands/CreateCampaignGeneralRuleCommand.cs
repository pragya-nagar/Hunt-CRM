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
    public class CreateCampaignGeneralRuleCommand : ICreateCampaignGeneralRuleCommand
    {
        private readonly IMapper _mapper;
        private readonly ISynergyContext _context;

        public CreateCampaignGeneralRuleCommand(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public void Dispatch(CreateRuleModel entity, Guid userId)
        {
            this.DispatchAsync(entity, userId).Wait();
        }

        public async Task<int> DispatchAsync(CreateRuleModel entity, Guid userId, CancellationToken cancellationToken = default)
        {
            var data = this._mapper.Map<CampaignRule>(entity);
            data.OnCreateAudit(userId);
            data.CampaignRuleItems.ToList().ForEach(a => a.OnCreateAudit(userId));

            this._context.CampaignRule.Add(data);
            return await this._context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}

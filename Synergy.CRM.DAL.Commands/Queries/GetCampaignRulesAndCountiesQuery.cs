using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synergy.CRM.DAL.Commands.Models.Results;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;
using Synergy.DataAccess.Context;

namespace Synergy.CRM.DAL.Commands.Queries
{
    public class GetCampaignRulesAndCountiesQuery : SingleQuery<Guid, CampaignRulesAndCountiesModel>
    {
        private readonly ISynergyContext _context;

        public GetCampaignRulesAndCountiesQuery(ISynergyContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override async Task<CampaignRulesAndCountiesModel> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Campaign
                .Include(x => x.CampaignCounty)
                .Include(x => x.CampaignRuleCampaign)
                .AsNoTracking()
                .Where(x => x.Id == id && x.DeletedOn == null)
                .Select(x => new CampaignRulesAndCountiesModel
                {
                    StateId = x.StateId,
                    RuleIds = x.CampaignRuleCampaign.Select(r => r.CampaignRuleId).ToList(),
                    CountyIds = x.CampaignCounty.Select(c => c.CountyId).ToList(),
                })
                .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}

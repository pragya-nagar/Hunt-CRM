using System;
using System.Collections.Generic;

namespace Synergy.CRM.DAL.Commands.Models
{
    public class AddRulesToCampaignModel
    {
        public Guid CampaignId { get; set; }

        public IEnumerable<Guid> CampaignRuleIds { get; set; }
    }
}

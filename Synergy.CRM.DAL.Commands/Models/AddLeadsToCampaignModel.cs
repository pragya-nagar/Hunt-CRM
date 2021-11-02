using System;
using System.Collections.Generic;

namespace Synergy.CRM.DAL.Commands.Models
{
    public class AddLeadsToCampaignModel
    {
        public Guid CampaignId { get; set; }

        public IEnumerable<Guid> CampaignLeadsIds { get; set; }
    }
}

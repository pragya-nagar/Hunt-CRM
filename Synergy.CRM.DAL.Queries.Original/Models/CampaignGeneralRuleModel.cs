using System;
using System.Collections.Generic;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Abstractions.Models;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class CampaignGeneralRuleModel : AuditModel, IModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<CampaignRuleItemModel> CampaignRuleItems { get; set; }
    }
}

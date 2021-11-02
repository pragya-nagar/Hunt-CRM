using System;
using System.Collections.Generic;
using Synergy.Common.DAL.Abstract;

namespace Synergy.CRM.DAL.Queries.Entities
{
    public class CampaignRuleItem : IAuditEntity<Guid>
    {
        public string Value { get; set; }

        public int CampaignLogicTypeId { get; set; }

        public CampaignLogicType CampaignLogicType { get; set; }

        public int CampaignRuleFieldId { get; set; }

        public CampaignRuleField CampaignRuleField { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

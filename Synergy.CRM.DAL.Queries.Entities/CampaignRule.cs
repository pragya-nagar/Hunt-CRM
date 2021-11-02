namespace Synergy.CRM.DAL.Queries.Entities
{
    using System;
    using System.Collections.Generic;
    using Synergy.Common.DAL.Abstract;

    public class CampaignRule : IAuditEntity<Guid>
    {
        public string Name { get; set; }

        public IEnumerable<CampaignRuleCampaign> CampaignLinks { get; set; }

        public IEnumerable<CampaignRuleItem> Items { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

namespace Synergy.CRM.DAL.Queries.Entities
{
    using System;
    using System.Collections.Generic;
    using Synergy.Common.DAL.Abstract;

    public class CampaignRuleCampaign : IAuditEntity<Guid>
    {
        public Guid CampaignRuleId { get; set; }

        public CampaignRule CampaignRule { get; set; }

        public Guid CampaignId { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

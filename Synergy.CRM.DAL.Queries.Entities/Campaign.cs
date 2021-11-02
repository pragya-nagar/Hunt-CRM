namespace Synergy.CRM.DAL.Queries.Entities
{
    using System;
    using System.Collections.Generic;
    using Synergy.Common.DAL.Abstract;

    public class Campaign : IAuditEntity<Guid>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string County { get; set; }

        public string Note { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? TargetDate { get; set; }

        public Guid AssignedUserId { get; set; }

        public User AssignedUser { get; set; }

        public int TypeId { get; set; }

        public CampaignType Type { get; set; }

        public int? SubTypeId { get; set; }

        public CampaignType SubType { get; set; }

        public IEnumerable<CampaignLead> LeadLinks { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

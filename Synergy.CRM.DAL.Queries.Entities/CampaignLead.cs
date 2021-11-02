namespace Synergy.CRM.DAL.Queries.Entities
{
    using System;
    using Synergy.Common.DAL.Abstract;

    public class CampaignLead : IAuditEntity<Guid>
    {
        public Guid LeadId { get; set; }

        public Lead Lead { get; set; }

        public Guid CampaignId { get; set; }

        public Campaign Campaign { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

using System;
using Synergy.Common.DAL.Abstract;

namespace Synergy.CRM.DAL.Queries.Entities
{
   public class OpportunityPropertyAudit : IAuditEntity<Guid>, IEntity
    {
        public Guid PropertyId { get; set; }

        public Guid OpportunityId { get; set; }

        public Opportunity Opportunity { get; set; }

        public Property Property { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }

        public DateTime InsertedOn { get; set; }

        public Guid InsertedBy { get; set; }
    }
}
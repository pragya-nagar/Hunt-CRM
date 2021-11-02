using System;
using Synergy.Common.DAL.Abstract;

namespace Synergy.CRM.DAL.Queries.Entities
{
    public class OpportunityPropertyType : IAuditEntity<int>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

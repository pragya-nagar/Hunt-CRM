using System;
using System.Collections.Generic;
using System.Text;
using Synergy.Common.DAL.Abstract;

namespace Synergy.CRM.DAL.Queries.Entities
{
    public class OpportunityPercentagePrepay : IAuditEntity<int>
    {
        public int Id { get; set; }

        public decimal PercentagePrepay { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

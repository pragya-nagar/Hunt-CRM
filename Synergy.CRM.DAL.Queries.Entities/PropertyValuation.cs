namespace Synergy.CRM.DAL.Queries.Entities
{
    using System;
    using Synergy.Common.DAL.Abstract;

    public class PropertyValuation : IAuditEntity<Guid>
    {
        public int AppraisedYear { get; set; }

        public decimal? AppraisedValue { get; set; }

        public bool IsActive { get; set; }

        public Guid PropertyId { get; set; }

        public Property Property { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

using System;
using Synergy.Common.DAL.Abstract;

namespace Synergy.CRM.DAL.Queries.Entities
{
    public class OpportunityBorrowerBase : IAuditEntity<Guid>, IEntity
    {
        public Guid OpportunityId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string CellPhone { get; set; }

        public string WorkPhone { get; set; }

        public string Fax { get; set; }

        public int Order { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

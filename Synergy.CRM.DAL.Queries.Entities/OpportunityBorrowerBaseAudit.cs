using System;
using Synergy.Common.DAL.Abstract;

namespace Synergy.CRM.DAL.Queries.Entities
{
    public class OpportunityBorrowerBaseAudit : IAuditEntity<Guid>, IEntity
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

        public Opportunity Opportunity { get; set; }

        public string Discriminator { get; set; }

        // OpportunityBorrowerAudit
        public string SSN { get; set; }

        public bool? IsMarried { get; set; }

        public string DateOfBirth { get; set; }

        // OpportunityCommercialBorrowerAudit
        public string EntityName { get; set; }

        public string TaxIdNumber { get; set; }

        public string Title { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }

        public DateTime InsertedOn { get; set; }
    }
}
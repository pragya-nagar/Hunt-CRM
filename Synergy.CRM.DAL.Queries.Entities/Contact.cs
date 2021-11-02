namespace Synergy.CRM.DAL.Queries.Entities
{
    using System;
    using Synergy.Common.DAL.Abstract;

    public class Contact : IAuditEntity<Guid>
    {
        public string Title { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string CellPhone { get; set; }

        public string OfficePhone { get; set; }

        public string Email { get; set; }

        public string MailingAddress1 { get; set; }

        public string MailingAddress2 { get; set; }

        public string MailingAddress3 { get; set; }

        public string MailingCity { get; set; }

        public string MailingZipCode { get; set; }

        public int? MailingStateId { get; set; }

        public State MailingState { get; set; }

        public int ContactTypeId { get; set; }

        public ContactType ContactType { get; set; }

        public Guid LeadId { get; set; }

        public Lead Lead { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

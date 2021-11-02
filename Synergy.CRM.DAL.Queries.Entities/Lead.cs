using System;
using System.Collections.Generic;
using Synergy.Common.DAL.Abstract;

namespace Synergy.CRM.DAL.Queries.Entities
{
    public class Lead : IAuditEntity<Guid>
    {
        public string AccountName { get; set; }

        public string MailingAddress1 { get; set; }

        public string MailingAddress2 { get; set; }

        public string MailingAddress3 { get; set; }

        public string MailingCity { get; set; }

        public string MailingZipCode { get; set; }

        public bool DoNotContact { get; set; }

        public decimal? TotalAmountDueProperties { get; set; }

        public int MailingStateId { get; set; }

        public State MailingState { get; set; }

        public IEnumerable<Contact> Contacts { get; set; }

        public IEnumerable<Property> Properties { get; set; }

        public IEnumerable<CampaignLead> CampaignLinks { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

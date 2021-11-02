using System;
using System.Collections.Generic;
using System.Linq;

namespace Synergy.CRM.Models
{
    public class CampaignLeadDumpModel
    {
        public CampaignLeadDumpModel()
            : this(1)
        {
        }

        public CampaignLeadDumpModel(int contactCount)
        {
            this.Contact = Enumerable.Repeat(new ContactDumpModel(), contactCount);
        }

        public string CampaignName { get; set; }

        public string CampaignType { get; set; }

        public string CampaignSubType { get; set; }

        public DateTime? CreateDate { get; set; }

        public string Description { get; set; }

        public DateTime? TargetDate { get; set; }

        public string Note { get; set; }

        public string AssignedUser { get; set; }

        public string AccountName { get; set; }

        public string MailingAddress1 { get; set; }

        public string MailingAddress2 { get; set; }

        public string MailingAddress3 { get; set; }

        public string MailingCity { get; set; }

        public string MailingState { get; set; }

        public string MailingZipCode { get; set; }

        public bool DoNotContact { get; set; }

        public decimal? AmountDue { get; set; }

        public IEnumerable<ContactDumpModel> Contact { get; set; }
    }
}
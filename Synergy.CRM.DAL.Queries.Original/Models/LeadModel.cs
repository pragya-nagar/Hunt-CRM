using System;
using System.Collections.Generic;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Abstractions.Models;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class LeadModel : AuditModel, IModel
    {
        public Guid Id { get; set; }

        public string AccountName { get; set; }

        public bool DoNotContact { get; set; }

        public int? PropertiesCount { get; set; }

        public decimal? TotalTaxDue { get; set; }

        public LeadAddressModel Address { get; set; }

        public List<ContactModel> Contacts { get; set; }
    }
}

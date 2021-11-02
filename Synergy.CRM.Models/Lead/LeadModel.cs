using System;
using Synergy.Common.Domain.Models.Abstracts;

namespace Synergy.CRM.Models
{
    public class LeadModel : IResultModel
    {
        public Guid Id { get; set; }

        public string AccountName { get; set; }

        public int? PropertiesCount { get; set; }

        public AddressModel Address { get; set; }

        public decimal? TotalTaxDue { get; set; }
    }
}
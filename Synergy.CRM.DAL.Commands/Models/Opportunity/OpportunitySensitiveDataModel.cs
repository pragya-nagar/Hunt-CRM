using System;

namespace Synergy.CRM.DAL.Commands.Models.Opportunity
{
    public class OpportunitySensitiveDataModel
    {
        public Guid Id { get; set; }

        public string SSN { get; set; }

        public DateTime? DayOfBirth { get; set; }

        public string TaxIdNumber { get; set; }

        public bool IsSSNChanged { get; set; }

        public bool IsDayOfBirthChanged { get; set; }

        public bool IsTaxIdNumberChanged { get; set; }
    }
}
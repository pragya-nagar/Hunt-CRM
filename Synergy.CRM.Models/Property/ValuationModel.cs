using System;

namespace Synergy.CRM.Models
{
    public class ValuationModel
    {
        public int Year { get; set; }

        public decimal LandValue { get; set; }

        public decimal ImprovementValue { get; set; }

        public decimal AppraisedValue { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
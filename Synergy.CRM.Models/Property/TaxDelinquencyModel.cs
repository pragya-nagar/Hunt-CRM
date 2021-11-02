using System.Collections.Generic;
using Synergy.CRM.Models.Property;

namespace Synergy.CRM.Models
{
    public class TaxDelinquencyModel
    {
        public int Year { get; set; }

        public List<CollectingEntityModel> CollectingEntities { get; set; }

        public decimal AmountDue { get; set; }
    }
}

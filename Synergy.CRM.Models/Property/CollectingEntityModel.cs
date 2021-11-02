using System;

namespace Synergy.CRM.Models.Property
{
    public class CollectingEntityModel
    {
        public Guid Id { get; set; }

        public int CollectingEntityTypeId { get; set; }

        public decimal AmountDue { get; set; }
    }
}

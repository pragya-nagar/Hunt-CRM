using System;
using System.Collections.Generic;
using Synergy.DataAccess.Abstractions.Interfaces;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class DelinquencyAmountDueModel : IModel
    {
        public Guid PropertyId { get; set; }

        public IEnumerable<DelinquencyAmountDueItem> Items { get; set; }
    }
}

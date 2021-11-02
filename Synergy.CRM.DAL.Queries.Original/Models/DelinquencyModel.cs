using System;
using System.Collections.Generic;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Abstractions.Models;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class DelinquencyModel : AuditModel, IModel
    {
        public Guid Id { get; set; }

        public int DelinquencyYear { get; set; }

        public decimal Amount { get; set; }

        public List<CollectingEntityModel> CollectingEntities { get; set; }
    }
}

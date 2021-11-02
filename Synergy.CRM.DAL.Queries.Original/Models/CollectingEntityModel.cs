using System;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Abstractions.Models;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class CollectingEntityModel : AuditModel, IModel
    {
        public Guid Id { get; set; }

        public int CollectingEntityTypeId { get; set; }

        public decimal AmountDue { get; set; }
    }
}

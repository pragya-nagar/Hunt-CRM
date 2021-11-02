using System;
using Synergy.DataAccess.Abstractions.Interfaces;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class PropertyApplyRulesModel : IModel
    {
        public Guid LeadId { get; set; }

        public decimal TotalAmountDue { get; set; }
    }
}

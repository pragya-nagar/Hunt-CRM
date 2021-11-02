using System.Collections.Generic;
using Synergy.DataAccess.Abstractions.Interfaces;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class CampaignRuleFieldModel : IModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<CampaignLogicTypeModel> LogicTypes { get; set; }
    }
}

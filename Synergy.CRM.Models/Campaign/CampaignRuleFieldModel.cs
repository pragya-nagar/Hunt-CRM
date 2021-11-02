namespace Synergy.CRM.Models
{
    using System.Collections.Generic;
    using Synergy.Common.Domain.Models.Abstracts;

    public class CampaignRuleFieldModel : IResultModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<CampaignLogicTypeModel> LogicTypes { get; set; }
    }
}

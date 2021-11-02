using System.Collections.Generic;
using Synergy.Common.Domain.Models.Abstracts;
using Synergy.Common.Domain.Models.Common;

namespace Synergy.CRM.Models
{
    public class CampaignTypeModel : IResultModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<FastEntityModel<int>> Children { get; set; }
    }
}

using System.Collections.Generic;
using Synergy.Common.Domain.Models.Common;

namespace Synergy.CRM.Models
{
    public class CampaignDetailsModel : CampaignModel
    {
        public List<FastEntityModel<int>> Counties { get; set; }

        public FastEntityModel<int> State { get; set; }

        public int TotalRecords { get; set; }

        public decimal TotalAmountRecords { get; set; }
    }
}
